using System.Text.Json;
using Core.Contracts.Enums;

namespace Core.Services.Payment.Webhooks;

/// <summary>
///     Verified provider webhook queued for processing
/// </summary>
public class PaymentWebhookMessage
{
    public PaymentProvider Provider { get; set; }
    public string ProviderTransactionId { get; set; } = null!;
    public JsonElement Payload { get; set; }
}