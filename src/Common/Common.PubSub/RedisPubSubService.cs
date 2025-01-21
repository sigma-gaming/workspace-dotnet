using System.Text.Json;
using Common.PubSub.Abstractions;
using StackExchange.Redis;

namespace Common.PubSub;

public class RedisPubSubService(IConnectionMultiplexer redis) : IPubSubService
{
    private readonly ISubscriber _subscriber = redis.GetSubscriber();
    
    public ITypedPubSub<T> Create<T>(string channelName) where T : class
        => new RedisTypedPubSub<T>(_subscriber, channelName);
}

file class RedisTypedPubSub<T>(ISubscriber subscriber, string channelName) : ITypedPubSub<T> where T : class
{
    private readonly RedisChannel _channel = new(channelName, RedisChannel.PatternMode.Literal);
    
    public async Task PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message);
        await subscriber.PublishAsync(_channel, json);
    }

    public IDisposable Subscribe(Action<T> handler)
    {
        var queue = subscriber.Subscribe(_channel);
        queue.OnMessage(message =>
        {
            if (message.Message.IsNullOrEmpty) return;
            var value = JsonSerializer.Deserialize<T>(message.Message!);
            if (value is not null)
                handler(value);
        });
        return new RedisSubscription(queue);
    }

    public IDisposable Subscribe(Func<T, Task> handler)
    {
        var queue = subscriber.Subscribe(_channel);
        queue.OnMessage(async message =>
        {
            if (message.Message.IsNullOrEmpty) return;
            var value = JsonSerializer.Deserialize<T>(message.Message!);
            if (value is not null)
                await handler(value);
        });
        return new RedisSubscription(queue);
    }
}

file class RedisSubscription(ChannelMessageQueue queue) : IDisposable
{
    public void Dispose() => queue.Unsubscribe();
} 