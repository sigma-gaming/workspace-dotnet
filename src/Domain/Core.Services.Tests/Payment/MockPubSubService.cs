using Common.PubSub.Abstractions;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text;
using System.Text.Json;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Mock implementation of IPubSubService for testing that works with the test NATS container
/// </summary>
public class MockPubSubService : IPubSubService
{
    private readonly IConnection _connection;
    private readonly ILogger _logger;

    public MockPubSubService(IConnection connection, ILogger logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public ITypedPubSub<T> Create<T>(string name) where T : class
    {
        _logger.LogInformation("Creating typed pubsub service of type {Type} with name {Name}", typeof(T).Name, name);
        return new MockTypedPubSub<T>(this, _connection, name, _logger);
    }

    public Task PublishAsync<T>(string subject, T data, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            _connection.Publish(subject, bytes);
            _logger.LogInformation("Published message to {Subject}: {Message}", subject, json);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to {Subject}", subject);
            throw;
        }
    }

    public IAsyncDisposable SubscribeAsync<T>(string subject, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var subscription = _connection.SubscribeAsync(subject, (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);
                    var data = JsonSerializer.Deserialize<T>(json);
                    if (data != null)
                    {
                        // Execute handler in fire-and-forget mode for simplicity in tests
                        _ = Task.Run(() => handler(data), cancellationToken);
                    }
                    _logger.LogInformation("Received message from {Subject}: {Message}", subject, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message from {Subject}", subject);
                }
            });

            _logger.LogInformation("Subscribed to {Subject}", subject);
            
            return new AsyncDisposableSubscription(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to {Subject}", subject);
            throw;
        }
    }

    private class AsyncDisposableSubscription : IAsyncDisposable
    {
        private readonly IAsyncSubscription _subscription;

        public AsyncDisposableSubscription(IAsyncSubscription subscription)
        {
            _subscription = subscription;
        }

        public ValueTask DisposeAsync()
        {
            _subscription.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

/// <summary>
/// Mock implementation of ITypedPubSub for testing
/// </summary>
public class MockTypedPubSub<T> : ITypedPubSub<T> where T : class
{
    private readonly IPubSubService _pubSubService;
    private readonly IConnection _connection;
    private readonly string _name;
    private readonly ILogger _logger;

    public MockTypedPubSub(IPubSubService pubSubService, IConnection connection, string name, ILogger logger)
    {
        _pubSubService = pubSubService;
        _connection = connection;
        _name = name;
        _logger = logger;
    }

    public Task PublishAsync(T data, CancellationToken cancellationToken = default)
    {
        var subject = $"{typeof(T).Name}.{_name}";
        _logger.LogInformation("Publishing typed message to {Subject}", subject);
        
        try
        {
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            _connection.Publish(subject, bytes);
            _logger.LogInformation("Published message to {Subject}: {Message}", subject, json);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to {Subject}", subject);
            throw;
        }
    }
    
    public Task TryPublishAsync(T data, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"{typeof(T).Name}.{_name}";
            var json = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            _connection.Publish(subject, bytes);
            _logger.LogInformation("TryPublished message to {Subject}: {Message}", subject, json);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to publish message: {Error}", ex.Message);
            // Swallow the exception but complete the task
            return Task.CompletedTask;
        }
    }

    public IAsyncDisposable SubscribeAsync(Func<T, Task> handler, CancellationToken cancellationToken = default)
    {
        var subject = $"{typeof(T).Name}.{_name}";
        _logger.LogInformation("Subscribing to typed messages on {Subject}", subject);
        
        try
        {
            var subscription = _connection.SubscribeAsync(subject, (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);
                    var data = JsonSerializer.Deserialize<T>(json);
                    if (data != null)
                    {
                        // Execute handler in fire-and-forget mode for simplicity in tests
                        _ = Task.Run(() => handler(data), cancellationToken);
                    }
                    _logger.LogInformation("Received message from {Subject}: {Message}", subject, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message from {Subject}", subject);
                }
            });

            _logger.LogInformation("Subscribed to {Subject}", subject);
            
            return new MockAsyncDisposableSubscription(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to {Subject}", subject);
            throw;
        }
    }
    
    public Task Subscribe(Func<T, Task> handler, CancellationToken cancellationToken = default)
    {
        var subject = $"{typeof(T).Name}.{_name}";
        _logger.LogInformation("Subscribing (sync) to typed messages on {Subject}", subject);
        
        try
        {
            _connection.SubscribeAsync(subject, (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);
                    var data = JsonSerializer.Deserialize<T>(json);
                    if (data != null)
                    {
                        // Execute handler in fire-and-forget mode for simplicity in tests
                        _ = Task.Run(() => handler(data), cancellationToken);
                    }
                    _logger.LogInformation("Received message from {Subject}: {Message}", subject, json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message from {Subject}", subject);
                }
            });

            _logger.LogInformation("Subscribed to {Subject}", subject);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to {Subject}", subject);
            throw;
        }
    }
    
    private class MockAsyncDisposableSubscription : IAsyncDisposable
    {
        private readonly IAsyncSubscription _subscription;

        public MockAsyncDisposableSubscription(IAsyncSubscription subscription)
        {
            _subscription = subscription;
        }

        public ValueTask DisposeAsync()
        {
            _subscription.Dispose();
            return ValueTask.CompletedTask;
        }
    }
} 