namespace Common.PubSub.Abstractions;

public interface IPubSubService
{
    ITypedPubSub<T> Create<T>(string channelName) where T : class;
}

public interface ITypedPubSub<T> where T : class
{
    Task PublishAsync(T message, CancellationToken cancellationToken = default);
    IDisposable Subscribe(Action<T> handler);
    IDisposable Subscribe(Func<T, Task> handler);
} 