using System.Text.Json;
using Core.Contracts.Enums;
using Core.RabbitMq.Configuration;
using Core.Services.Outbox;

namespace Core.Services.Payment.Webhooks;

/// <summary>
///     Queues verified webhooks through the outbox, so the provider gets a response before processing
/// </summary>
public class PaymentWebhookOutbox(OutboxService outbox, RabbitMqOptions rabbitMqOptions)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Guid> EnqueueAsync<TRequest>(
        PaymentProvider provider,
        string providerTransactionId,
        TRequest request,
        CancellationToken ct = default)
    {
        var message = new PaymentWebhookMessage
        {
            Provider = provider,
            ProviderTransactionId = providerTransactionId,
            Payload = JsonSerializer.SerializeToElement(request, JsonOptions),
        };

        var queue = rabbitMqOptions.PaymentWebhooks;
        return await outbox.EnqueueAsync(queue.Exchange, queue.RoutingKey, message, ct);
    }
}