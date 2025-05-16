using RabbitMQ.Client;

namespace Common.RabbitMq;

public static class RabbitMqTopology
{
    public static async Task DeclareAsync(IChannel channel, RabbitMqQueueOptions options, CancellationToken ct = default)
    {
        await channel.ExchangeDeclareAsync(options.Exchange, ExchangeType.Direct, true, cancellationToken: ct);
        await channel.ExchangeDeclareAsync(options.RetryExchange, ExchangeType.Direct, true, cancellationToken: ct);
        await channel.ExchangeDeclareAsync(options.DeadLetterExchange, ExchangeType.Direct, true, cancellationToken: ct);

        // Rejected messages go to the retry queue and come back after the TTL
        await channel.QueueDeclareAsync(options.Queue, true, false, false, new Dictionary<string, object?>
        {
            ["x-queue-type"] = "quorum",
            ["x-dead-letter-exchange"] = options.RetryExchange,
            ["x-dead-letter-strategy"] = "at-least-once",
            ["x-overflow"] = "reject-publish",
        }, cancellationToken: ct);

        await channel.QueueDeclareAsync(options.RetryQueue, true, false, false, new Dictionary<string, object?>
        {
            ["x-queue-type"] = "quorum",
            ["x-message-ttl"] = (long)options.RetryDelay.TotalMilliseconds,
            ["x-dead-letter-exchange"] = options.Exchange,
            ["x-dead-letter-strategy"] = "at-least-once",
            ["x-overflow"] = "reject-publish",
        }, cancellationToken: ct);

        await channel.QueueDeclareAsync(options.DeadLetterQueue, true, false, false, new Dictionary<string, object?>
        {
            ["x-queue-type"] = "quorum",
        }, cancellationToken: ct);

        await channel.QueueBindAsync(options.Queue, options.Exchange, options.RoutingKey, cancellationToken: ct);
        await channel.QueueBindAsync(options.RetryQueue, options.RetryExchange, options.RoutingKey, cancellationToken: ct);
        await channel.QueueBindAsync(options.DeadLetterQueue, options.DeadLetterExchange, options.RoutingKey,
            cancellationToken: ct);
    }
}