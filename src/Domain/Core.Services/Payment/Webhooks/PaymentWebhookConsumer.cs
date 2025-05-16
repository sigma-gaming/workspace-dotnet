using Common.RabbitMq;
using Core.RabbitMq.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment.Webhooks;

/// <summary>
///     Consumes queued webhooks, failed ones are retried through the retry queue and end up in the dead letter queue
/// </summary>
public class PaymentWebhookConsumer(
    RabbitMqConnection connection,
    RabbitMqOptions rabbitMqOptions,
    IServiceScopeFactory scopeFactory,
    ILogger<PaymentWebhookConsumer> logger)
    : RabbitMqConsumer<PaymentWebhookMessage>(connection, rabbitMqOptions.PaymentWebhooks, logger)
{
    protected override async Task<bool> HandleAsync(PaymentWebhookMessage message, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<PaymentWebhookProcessor>();

        return await processor.ProcessAsync(message);
    }
}