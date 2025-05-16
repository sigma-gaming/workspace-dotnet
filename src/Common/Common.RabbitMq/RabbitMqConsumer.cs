using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Common.RabbitMq;

public abstract class RabbitMqConsumer<TMessage>(
    RabbitMqConnection connection,
    RabbitMqQueueOptions options,
    ILogger logger)
    : BackgroundService where TMessage : class
{
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);
    private IChannel? channel;
    private string? consumerTag;
    private int inFlight;

    protected abstract Task<bool> HandleAsync(TMessage message, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await StartConsumingAsync(stoppingToken);
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Failed to start consuming {Queue}, retrying in 5 seconds", options.Queue);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }

    private async Task StartConsumingAsync(CancellationToken stoppingToken)
    {
        var amqpConnection = await connection.GetConnectionAsync(stoppingToken);
        if (channel is not null) await channel.DisposeAsync();

        channel = await amqpConnection.CreateChannelAsync(new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true,
            consumerDispatchConcurrency: options.Concurrency), stoppingToken);

        await channel.BasicQosAsync(0, options.PrefetchCount, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, args) => OnReceivedAsync(args, stoppingToken);

        consumerTag = await channel.BasicConsumeAsync(options.Queue, false, consumer, stoppingToken);

        logger.LogInformation(
            "Consuming {Queue} with prefetch {PrefetchCount} and concurrency {Concurrency}",
            options.Queue, options.PrefetchCount, options.Concurrency);
    }

    private async Task OnReceivedAsync(BasicDeliverEventArgs args, CancellationToken stoppingToken)
    {
        Interlocked.Increment(ref inFlight);
        try
        {
            await ProcessAsync(args, stoppingToken);
        }
        catch (OperationInterruptedException ex)
        {
            // Not acked, the broker redelivers it after the channel is recovered
            logger.LogWarning(ex, "Channel closed before message {MessageId} was settled",
                args.BasicProperties.MessageId);
        }
        finally
        {
            Interlocked.Decrement(ref inFlight);
        }
    }

    private async Task ProcessAsync(BasicDeliverEventArgs args, CancellationToken stoppingToken)
    {
        var attempt = GetAttempt(args.BasicProperties);

        TMessage? message = null;
        try
        {
            message = JsonSerializer.Deserialize<TMessage>(args.Body.Span, jsonOptions);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid message {MessageId} in {Queue}", args.BasicProperties.MessageId,
                options.Queue);
        }

        if (message is null)
        {
            await DeadLetterAsync(args, attempt, "Invalid payload");
            return;
        }

        var handled = false;
        string? error = null;
        try
        {
            handled = await HandleAsync(message, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Not acked, the broker redelivers it to another consumer
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle message {MessageId} from {Queue}", args.BasicProperties.MessageId,
                options.Queue);
            error = ex.Message;
        }

        if (handled)
        {
            await channel!.BasicAckAsync(args.DeliveryTag, false);
            RabbitMqMetrics.Consumed.WithLabels(options.Queue).Inc();
            return;
        }

        if (attempt >= options.MaxAttempts)
        {
            await DeadLetterAsync(args, attempt, error ?? "Handler returned false");
            return;
        }

        logger.LogWarning("Message {MessageId} from {Queue} failed on attempt {Attempt}, retrying in {Delay}",
            args.BasicProperties.MessageId, options.Queue, attempt, options.RetryDelay);

        await channel!.BasicNackAsync(args.DeliveryTag, false, false);
        RabbitMqMetrics.Retried.WithLabels(options.Queue).Inc();
    }

    private async Task DeadLetterAsync(BasicDeliverEventArgs args, int attempt, string error)
    {
        var headers = args.BasicProperties.Headers is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(args.BasicProperties.Headers);

        headers["x-attempts"] = attempt;
        headers["x-error"] = error;

        var properties = new BasicProperties(args.BasicProperties)
        {
            Headers = headers,
        };

        try
        {
            await channel!.BasicPublishAsync(options.DeadLetterExchange, options.RoutingKey, true, properties,
                args.Body);
            await channel.BasicAckAsync(args.DeliveryTag, false);
            RabbitMqMetrics.DeadLettered.WithLabels(options.Queue).Inc();

            logger.LogError("Message {MessageId} from {Queue} moved to {DeadLetterQueue} after {Attempt} attempts: {Error}",
                args.BasicProperties.MessageId, options.Queue, options.DeadLetterQueue, attempt, error);
        }
        catch (PublishException ex)
        {
            logger.LogError(ex, "Failed to dead letter message {MessageId}", args.BasicProperties.MessageId);
            await channel!.BasicNackAsync(args.DeliveryTag, false, false);
        }
    }

    private int GetAttempt(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is null ||
            !properties.Headers.TryGetValue("x-death", out var value) ||
            value is not IList<object> deaths)
            return 1;

        long rejected = 0;
        foreach (var death in deaths.OfType<IDictionary<string, object?>>())
            if (death.TryGetValue("queue", out var queue) && queue is byte[] name &&
                Encoding.UTF8.GetString(name) == options.Queue &&
                death.TryGetValue("count", out var count) && count is long times)
                rejected += times;

        return (int)rejected + 1;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop deliveries and let in-flight messages finish before the channel is closed
        if (channel is { IsOpen: true } && consumerTag is not null)
            await channel.BasicCancelAsync(consumerTag, cancellationToken: cancellationToken);

        while (Volatile.Read(ref inFlight) > 0 && !cancellationToken.IsCancellationRequested)
            await Task.Delay(50, CancellationToken.None);

        await base.StopAsync(cancellationToken);
        if (channel is not null) await channel.CloseAsync();
    }

    public override void Dispose()
    {
        channel?.Dispose();
        base.Dispose();
    }
}