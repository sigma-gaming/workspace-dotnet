using System.Text;
using Common.Db.Extensions;
using Common.RabbitMq;
using Core.Contracts.Entities;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Outbox;

/// <summary>
///     Publishes outbox messages to RabbitMQ and marks them as published after the broker confirms them
/// </summary>
public class OutboxPublisherService(
    IDbContextFactory<CoreDbContext> dbFactory,
    RabbitMqPublisher publisher,
    OutboxSignal signal,
    IOptions<OutboxOptions> options,
    ILogger<OutboxPublisherService> logger)
    : BackgroundService
{
    private readonly OutboxOptions options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var published = await PublishBatchAsync(stoppingToken);
                if (published < options.BatchSize) await signal.WaitAsync(options.PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                // Messages stay in the outbox until RabbitMQ is available again
                logger.LogError(ex, "Failed to publish outbox messages, retrying in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }

    public async Task<int> PublishBatchAsync(CancellationToken ct = default)
    {
        await publisher.ConnectAsync(ct);
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        return await db.ExecuteInTransactionAsync(async () =>
        {
            // SKIP LOCKED lets several replicas publish without picking the same rows
            var messages = await db.OutboxMessages
                .FromSql($"""
                          SELECT * FROM outbox_message
                          WHERE published_at IS NULL
                          ORDER BY created_at
                          LIMIT {options.BatchSize}
                          FOR UPDATE SKIP LOCKED
                          """)
                .ToListAsync(ct);

            if (messages.Count == 0) return 0;

            var results = await Task.WhenAll(messages.Select(message => TryPublishAsync(message, ct)));

            var publishedAt = DateTime.UtcNow;
            for (var i = 0; i < messages.Count; i++)
                if (results[i])
                    messages[i].PublishedAt = publishedAt;

            await db.SaveChangesAsync(ct);

            return results.Count(result => result);
        });
    }

    private async Task<bool> TryPublishAsync(OutboxMessageEntity message, CancellationToken ct)
    {
        try
        {
            await publisher.PublishAsync(
                message.Exchange,
                message.RoutingKey,
                Encoding.UTF8.GetBytes(message.Payload),
                message.Id.ToString(),
                ct);

            return true;
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Failed to publish outbox message {MessageId}", message.Id);
            return false;
        }
    }
}