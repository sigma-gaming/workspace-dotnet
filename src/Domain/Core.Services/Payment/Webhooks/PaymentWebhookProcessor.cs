using System.Text.Json;
using Core.Contracts.Enums;
using Core.Services.Payment.Providers.CryptoCloud;
using Core.Services.Payment.Providers.Plisio;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment.Webhooks;

/// <summary>
///     Processes queued webhooks with the provider webhook handlers
/// </summary>
public class PaymentWebhookProcessor(
    PlisioWebhookHandler plisioWebhookHandler,
    CryptoCloudWebhookHandler cryptoCloudWebhookHandler,
    ILogger<PaymentWebhookProcessor> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<bool> ProcessAsync(PaymentWebhookMessage message)
    {
        logger.LogInformation("Processing {Provider} webhook for transaction {TransactionId}",
            message.Provider, message.ProviderTransactionId);

        switch (message.Provider)
        {
            case PaymentProvider.Plisio:
                var plisioRequest = message.Payload.Deserialize<PlisioWebhookRequest>(JsonOptions);
                return plisioRequest is not null && await plisioWebhookHandler.HandleWebhookAsync(plisioRequest);

            case PaymentProvider.CryptoCloud:
                var cryptoCloudRequest = message.Payload.Deserialize<CryptoCloudWebhookRequest>(JsonOptions);
                return cryptoCloudRequest is not null &&
                       await cryptoCloudWebhookHandler.HandleWebhookAsync(cryptoCloudRequest);

            default:
                logger.LogWarning("Unknown webhook provider: {Provider}", message.Provider);
                return false;
        }
    }
}