using RabbitMQ.Client;

namespace Common.RabbitMq;

public class RabbitMqPublisher(RabbitMqConnection connection) : IAsyncDisposable
{
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private IChannel? channel;

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        await GetChannelAsync(ct);
    }

    // Completes when the broker confirms the message, throws if it was nacked or returned as unroutable
    public async Task PublishAsync(
        string exchange,
        string routingKey,
        ReadOnlyMemory<byte> body,
        string messageId,
        CancellationToken ct = default)
    {
        var publishChannel = await GetChannelAsync(ct);

        var properties = new BasicProperties
        {
            MessageId = messageId,
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
        };

        await publishChannel.BasicPublishAsync(exchange, routingKey, true, properties, body, ct);
        RabbitMqMetrics.Published.WithLabels(exchange).Inc();
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken ct)
    {
        if (channel is { IsOpen: true }) return channel;

        await semaphore.WaitAsync(ct);
        try
        {
            if (channel is { IsOpen: true }) return channel;
            if (channel is not null) await channel.DisposeAsync();

            var amqpConnection = await connection.GetConnectionAsync(ct);
            channel = await amqpConnection.CreateChannelAsync(new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true), ct);

            return channel;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (channel is not null) await channel.DisposeAsync();
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}