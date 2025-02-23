namespace Common.PubSub.Abstractions;

public interface IPubSubService
{
    ITypedPubSub<T> Create<T>(string channelName) where T : class;
}

public interface ITypedPubSub<T> where T : class
{
    Task PublishAsync(T message, CancellationToken ct = default);
    Task TryPublishAsync(T message, CancellationToken ct = default);
    Task Subscribe(Func<T, Task> handler);
}