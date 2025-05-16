using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Common.RabbitMq;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.RabbitMq.Configuration;
using Core.Services.Outbox;
using Core.Services.Payment;
using Core.Services.Payment.Providers.CryptoCloud;
using Core.Services.Payment.Providers.Plisio;
using Core.Services.Payment.Webhooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Xunit;

namespace Core.Services.Tests.Payment;

public class PaymentWebhookQueueTests(
    PaymentDbFixture dbFixture,
    PaymentCacheFixture cacheFixture,
    PaymentNatsFixture natsFixture,
    PaymentRabbitMqFixture rabbitMqFixture)
    : PaymentIntegrationTestBase(dbFixture, cacheFixture, natsFixture)
{
    private const string PlisioApiKey = "test-plisio-api-key";

    private readonly RabbitMqOptions rabbitMqOptions = new()
    {
        PaymentWebhooks = new RabbitMqQueueOptions
        {
            Exchange = $"payments-{Guid.NewGuid():N}",
            Queue = $"payment-webhooks-{Guid.NewGuid():N}",
            RoutingKey = "payment.webhook",
            PrefetchCount = 10,
            Concurrency = 1,
            MaxAttempts = 3,
            RetryDelay = TimeSpan.FromMilliseconds(300),
        },
    };

    private RabbitMqQueueOptions Queue => rabbitMqOptions.PaymentWebhooks;

    protected override async Task ConfigureServicesAsync(IServiceCollection services)
    {
        await base.ConfigureServicesAsync(services);

        services.AddSingleton(rabbitMqOptions);
        services.AddSingleton(rabbitMqOptions.PaymentWebhooks);
        services.AddSingleton(rabbitMqFixture.CreateConnectionFactory());
        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<RabbitMqPublisher>();

        services.AddSingleton(DbFixture.DbContextFactory);
        services.AddSingleton(Options.Create(new OutboxOptions()));
        services.AddSingleton<OutboxSignal>();
        services.AddScoped<OutboxService>();
        services.AddSingleton<OutboxPublisherService>();

        services.AddSingleton(Options.Create(new PlisioOptions { ApiKey = PlisioApiKey }));
        services.AddScoped<PlisioWebhookVerifier>();
        services.AddScoped<PaymentWebhookOutbox>();
        services.AddScoped<PaymentWebhookHandler>();
        services.AddScoped<PlisioWebhookHandler>();
        services.AddScoped<CryptoCloudWebhookHandler>();
        services.AddScoped<PaymentWebhookProcessor>();
        services.AddSingleton<PaymentWebhookConsumer>();
    }

    [Fact]
    public void Should_VerifyPlisioSignature()
    {
        var verifier = GetService<PlisioWebhookVerifier>();
        var request = CreatePlisioRequest("txn-signature");

        var signature = Sign(request);
        Assert.True(verifier.VerifySignature(request, signature));

        request.Amount = "100";
        Assert.False(verifier.VerifySignature(request, signature));
    }

    [Fact]
    public async Task Should_PublishOutboxMessage_WithConfirm_And_MarkItPublished()
    {
        await GetService<RabbitMqConnection>().GetConnectionAsync();

        var request = CreatePlisioRequest("txn-outbox");
        var messageId = await GetService<PaymentWebhookOutbox>()
            .EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);

        var published = await GetService<OutboxPublisherService>().PublishBatchAsync();
        Assert.True(published >= 1);

        await using var db = await DbFixture.CreateDbContextAsync();
        var outboxMessage = await db.OutboxMessages.SingleAsync(m => m.Id == messageId);
        Assert.NotNull(outboxMessage.PublishedAt);

        await using var connection = await rabbitMqFixture.CreateConnectionFactory().CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var delivery = await channel.BasicGetAsync(Queue.Queue, true);

        Assert.NotNull(delivery);
        Assert.Equal(messageId.ToString(), delivery.BasicProperties.MessageId);

        var message = JsonSerializer.Deserialize<PaymentWebhookMessage>(delivery.Body.Span,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(message);
        Assert.Equal(PaymentProvider.Plisio, message.Provider);
        Assert.Equal("txn-outbox", message.ProviderTransactionId);
    }

    [Fact]
    public async Task Should_KeepOutboxMessage_When_BrokerIsUnavailable()
    {
        var request = CreatePlisioRequest("txn-broker-down");
        var messageId = await GetService<PaymentWebhookOutbox>()
            .EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);

        var unavailable = new RabbitMqConnection(
            new ConnectionFactory { HostName = "localhost", Port = 1 },
            [Queue],
            NullLogger<RabbitMqConnection>.Instance);

        var publisher = new OutboxPublisherService(
            DbFixture.DbContextFactory,
            new RabbitMqPublisher(unavailable),
            new OutboxSignal(),
            Options.Create(new OutboxOptions()),
            NullLogger<OutboxPublisherService>.Instance);

        await Assert.ThrowsAnyAsync<Exception>(() => publisher.PublishBatchAsync());

        await using var db = await DbFixture.CreateDbContextAsync();
        var outboxMessage = await db.OutboxMessages.SingleAsync(m => m.Id == messageId);
        Assert.Null(outboxMessage.PublishedAt);

        // Published once the broker is back
        await GetService<OutboxPublisherService>().PublishBatchAsync();
        await db.Entry(outboxMessage).ReloadAsync();
        Assert.NotNull(outboxMessage.PublishedAt);
    }

    [Fact]
    public async Task Should_CreditBalanceOnce_When_WebhookIsDeliveredTwice()
    {
        var deposit = await CreatePendingDepositAsync(1000);
        var request = CreatePlisioRequest(deposit.ProviderTransactionId);

        // The provider retries the webhook, both copies pass the signature check
        var outbox = GetService<PaymentWebhookOutbox>();
        await outbox.EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);
        await outbox.EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);

        await GetService<RabbitMqConnection>().GetConnectionAsync();
        await GetService<OutboxPublisherService>().PublishBatchAsync();

        var consumer = GetService<PaymentWebhookConsumer>();
        await consumer.StartAsync(CancellationToken.None);
        await WaitForAsync(() => RabbitMqMetrics.Consumed.WithLabels(Queue.Queue).Value >= 2);
        await consumer.StopAsync(CancellationToken.None);

        await using var db = await DbFixture.CreateDbContextAsync();
        var updatedDeposit = await db.Deposits.SingleAsync(d => d.Id == deposit.Id);
        var balance = await db.Balances.SingleAsync(b => b.UserId == deposit.UserId);
        var deposits = await db.Transactions.CountAsync(t =>
            t.UserId == deposit.UserId && t.Type == TransactionType.Deposit);

        Assert.Equal(PaymentStatus.Completed, updatedDeposit.Status);
        Assert.Equal(1000, balance.Available);
        Assert.Equal(1, deposits);
        Assert.Equal(0, RabbitMqMetrics.Retried.WithLabels(Queue.Queue).Value);
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(Queue.DeadLetterQueue));
    }

    [Fact]
    public async Task Should_DeadLetterWebhook_When_DepositIsNotFound()
    {
        var request = CreatePlisioRequest($"missing-{Guid.NewGuid():N}");
        await GetService<PaymentWebhookOutbox>().EnqueueAsync(PaymentProvider.Plisio, request.TxnId, request);

        await GetService<RabbitMqConnection>().GetConnectionAsync();
        await GetService<OutboxPublisherService>().PublishBatchAsync();

        var consumer = GetService<PaymentWebhookConsumer>();
        await consumer.StartAsync(CancellationToken.None);
        await WaitForAsync(() => RabbitMqMetrics.DeadLettered.WithLabels(Queue.Queue).Value >= 1);
        await consumer.StopAsync(CancellationToken.None);

        Assert.Equal(Queue.MaxAttempts - 1, RabbitMqMetrics.Retried.WithLabels(Queue.Queue).Value);
        Assert.Equal(1u, await rabbitMqFixture.GetMessageCountAsync(Queue.DeadLetterQueue));
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(Queue.Queue));
    }

    private async Task<DepositEntity> CreatePendingDepositAsync(long gemAmount)
    {
        var user = await CreateTestUserAsync();

        var deposit = new DepositEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            GemAmount = gemAmount,
            Currency = Currency.Ton,
            Method = DepositMethod.Crypto,
            Provider = PaymentProvider.Plisio,
            Status = PaymentStatus.Pending,
            ProviderTransactionId = Guid.NewGuid().ToString(),
            ProviderAmount = "1.0",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FlowDetails = new DepositFlowDetailsRedirect { RedirectUrl = "https://example.com/redirect" },
        };

        DbContext.Deposits.Add(deposit);
        await DbContext.SaveChangesAsync();

        Logger.LogInformation("Created pending deposit {DepositId}", deposit.Id);
        return deposit;
    }

    private static PlisioWebhookRequest CreatePlisioRequest(string txnId)
    {
        return new PlisioWebhookRequest
        {
            Type = "invoice",
            TxnId = txnId,
            Status = "completed",
            Amount = "1.5",
            Currency = "TON",
        };
    }

    private static string Sign(PlisioWebhookRequest request)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes($"{request.TxnId}:{request.Status}:{request.Amount}:{PlisioApiKey}"));
        return Convert.ToHexStringLower(hash);
    }

    private static async Task WaitForAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (!condition())
        {
            if (DateTime.UtcNow > deadline) throw new TimeoutException("Condition was not met in time");
            await Task.Delay(100);
        }
    }
}