using System.Text;
using Common.RabbitMq;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using Xunit;

namespace Core.Services.Tests.Payment;

[Collection("PaymentTestCollection")]
public class RabbitMqConsumerTests(PaymentRabbitMqFixture rabbitMqFixture)
{
    [Fact]
    public async Task Should_RetryThroughRetryQueue_And_DeadLetter_AfterMaxAttempts()
    {
        var options = CreateQueueOptions(3);
        await using var connection = await CreateConnectionAsync(options);

        var attempts = 0;
        var consumer = new TestConsumer(connection, options, (_, _) =>
        {
            Interlocked.Increment(ref attempts);
            return Task.FromResult(false);
        });

        await consumer.StartAsync(CancellationToken.None);
        await PublishAsync(connection, options, Encoding.UTF8.GetBytes("{\"id\":\"1\"}"));

        var deadLetter = await WaitForMessageAsync(options.DeadLetterQueue);
        await consumer.StopAsync(CancellationToken.None);

        Assert.NotNull(deadLetter);
        Assert.Equal(3, attempts);
        Assert.Equal(3, deadLetter.BasicProperties.Headers?["x-attempts"]);
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(options.Queue));
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(options.RetryQueue));
    }

    [Fact]
    public async Task Should_AckMessage_When_RetrySucceeds()
    {
        var options = CreateQueueOptions(5);
        await using var connection = await CreateConnectionAsync(options);

        var attempts = 0;
        var handled = new TaskCompletionSource<TestMessage>();
        var consumer = new TestConsumer(connection, options, (message, _) =>
        {
            if (Interlocked.Increment(ref attempts) < 2) throw new InvalidOperationException("Database is down");

            handled.TrySetResult(message);
            return Task.FromResult(true);
        });

        await consumer.StartAsync(CancellationToken.None);
        await PublishAsync(connection, options, Encoding.UTF8.GetBytes("{\"id\":\"2\"}"));

        var message = await handled.Task.WaitAsync(TimeSpan.FromSeconds(15));
        await Task.Delay(500);
        await consumer.StopAsync(CancellationToken.None);

        Assert.Equal("2", message.Id);
        Assert.Equal(2, attempts);
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(options.Queue));
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(options.DeadLetterQueue));
    }

    [Fact]
    public async Task Should_DeadLetterInvalidPayload_WithoutRetries()
    {
        var options = CreateQueueOptions(5);
        await using var connection = await CreateConnectionAsync(options);

        var attempts = 0;
        var consumer = new TestConsumer(connection, options, (_, _) =>
        {
            Interlocked.Increment(ref attempts);
            return Task.FromResult(true);
        });

        await consumer.StartAsync(CancellationToken.None);
        await PublishAsync(connection, options, Encoding.UTF8.GetBytes("not a json"));

        var deadLetter = await WaitForMessageAsync(options.DeadLetterQueue);
        await consumer.StopAsync(CancellationToken.None);

        Assert.NotNull(deadLetter);
        Assert.Equal(0, attempts);
        Assert.Equal(1, deadLetter.BasicProperties.Headers?["x-attempts"]);
    }

    [Fact]
    public async Task Should_RedeliverUnackedMessage_When_ConsumerStops()
    {
        var options = CreateQueueOptions(5);
        await using var connection = await CreateConnectionAsync(options);

        var started = new TaskCompletionSource();
        var blocking = new TestConsumer(connection, options, async (_, ct) =>
        {
            started.TrySetResult();
            await Task.Delay(Timeout.Infinite, ct);
            return true;
        });

        await blocking.StartAsync(CancellationToken.None);
        await PublishAsync(connection, options, Encoding.UTF8.GetBytes("{\"id\":\"3\"}"));
        await started.Task.WaitAsync(TimeSpan.FromSeconds(15));

        // The handler does not finish within the shutdown timeout, the message is returned to the queue unacked
        using (var shutdownTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            await blocking.StopAsync(shutdownTimeout.Token);

        var handled = new TaskCompletionSource<TestMessage>();
        var consumer = new TestConsumer(connection, options, (message, _) =>
        {
            handled.TrySetResult(message);
            return Task.FromResult(true);
        });

        await consumer.StartAsync(CancellationToken.None);
        var message = await handled.Task.WaitAsync(TimeSpan.FromSeconds(15));
        await consumer.StopAsync(CancellationToken.None);

        Assert.Equal("3", message.Id);
    }

    [Fact]
    public async Task Should_FinishInFlightMessage_BeforeStopping()
    {
        var options = CreateQueueOptions(5);
        await using var connection = await CreateConnectionAsync(options);

        var started = new TaskCompletionSource();
        var handled = 0;
        var consumer = new TestConsumer(connection, options, async (_, _) =>
        {
            started.TrySetResult();
            await Task.Delay(500);
            Interlocked.Increment(ref handled);
            return true;
        });

        await consumer.StartAsync(CancellationToken.None);
        await PublishAsync(connection, options, Encoding.UTF8.GetBytes("{\"id\":\"4\"}"));
        await started.Task.WaitAsync(TimeSpan.FromSeconds(15));

        await consumer.StopAsync(CancellationToken.None);

        Assert.Equal(1, handled);
        Assert.Equal(0u, await rabbitMqFixture.GetMessageCountAsync(options.Queue));
    }

    private static RabbitMqQueueOptions CreateQueueOptions(int maxAttempts)
    {
        var name = $"test-{Guid.NewGuid():N}";

        return new RabbitMqQueueOptions
        {
            Exchange = name,
            Queue = name,
            RoutingKey = "test",
            PrefetchCount = 10,
            Concurrency = 1,
            MaxAttempts = maxAttempts,
            RetryDelay = TimeSpan.FromMilliseconds(300),
        };
    }

    private async Task<RabbitMqConnection> CreateConnectionAsync(RabbitMqQueueOptions options)
    {
        var connection = new RabbitMqConnection(
            rabbitMqFixture.CreateConnectionFactory(),
            [options],
            NullLogger<RabbitMqConnection>.Instance);

        await connection.GetConnectionAsync();
        return connection;
    }

    private static async Task PublishAsync(RabbitMqConnection connection, RabbitMqQueueOptions options, byte[] body)
    {
        await using var publisher = new RabbitMqPublisher(connection);
        await publisher.PublishAsync(options.Exchange, options.RoutingKey, body, Guid.NewGuid().ToString());
    }

    private async Task<BasicGetResult?> WaitForMessageAsync(string queue)
    {
        await using var connection = await rabbitMqFixture.CreateConnectionFactory().CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < deadline)
        {
            var result = await channel.BasicGetAsync(queue, true);
            if (result is not null) return result;

            await Task.Delay(100);
        }

        return null;
    }
}

public class TestMessage
{
    public string Id { get; set; } = null!;
}

internal class TestConsumer(
    RabbitMqConnection connection,
    RabbitMqQueueOptions options,
    Func<TestMessage, CancellationToken, Task<bool>> handler)
    : RabbitMqConsumer<TestMessage>(connection, options, NullLogger.Instance)
{
    protected override Task<bool> HandleAsync(TestMessage message, CancellationToken ct)
    {
        return handler(message, ct);
    }
}