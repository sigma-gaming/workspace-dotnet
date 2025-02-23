using System.Text.Json;
using Common.PubSub.Abstractions;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;

namespace Common.PubSub;

public class NatsPubSubService(INatsConnection connection, ILogger<NatsPubSubService> logger) : IPubSubService
{
    public ITypedPubSub<T> Create<T>(string channelName) where T : class
    {
        return new NatsTypedPubSub<T>(connection, logger, channelName);
    }
}

file class NatsTypedPubSub<T>(
    INatsConnection connection,
    ILogger logger,
    string subject
)
    : ITypedPubSub<T> where T : class
{
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishAsync(T message, CancellationToken ct = default)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
        await connection.PublishAsync(subject, data, cancellationToken: ct);
    }

    public async Task TryPublishAsync(T message, CancellationToken ct = default)
    {
        if (connection.ConnectionState != NatsConnectionState.Open) return;

        try
        {
            await PublishAsync(message, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing message to {Subject}", subject);
        }
    }

    public async Task Subscribe(Func<T, Task> handler)
    {
        try
        {
            await foreach (var message in connection.SubscribeAsync<byte[]>(subject))
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Data, jsonOptions);
                    logger.LogInformation("Received message {Data}", data);
                    if (data is not null) await handler(data);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message from {Subject}", subject);
                }
        }
        catch
        {
            logger.LogError("Error subscribing to {Subject}", subject);
            await Task.Delay(TimeSpan.FromSeconds(5));
            await Subscribe(handler);
        }
    }
}