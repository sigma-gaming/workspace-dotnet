using Core.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Handler for Plisio webhooks
/// </summary>
public class PlisioWebhookHandler(
    PaymentWebhookHandler webhookHandler,
    ILogger<PlisioWebhookHandler> logger)
{
    /// <summary>
    ///     Handles a webhook notification from Plisio, the signature is verified by <see cref="PlisioWebhookVerifier" />
    /// </summary>
    public async Task<bool> HandleWebhookAsync(PlisioWebhookRequest request)
    {
        try
        {
            // Map the status
            var status = PlisioStatusMapper.MapInvoiceStatus(request.Status);

            // Handle the webhook based on the type
            if (request.Type == "deposit" || request.Type == "invoice")
                return await webhookHandler.HandleDepositWebhookAsync(
                    request.TxnId,
                    status,
                    PaymentProvider.Plisio);

            if (request.Type == "withdrawal" || request.Type == "cash_out")
                return await webhookHandler.HandleWithdrawalWebhookAsync(
                    request.TxnId,
                    status,
                    PaymentProvider.Plisio);

            logger.LogWarning("Unknown webhook type: {Type}", request.Type);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle Plisio webhook");
            return false;
        }
    }
}

/// <summary>
///     Plisio webhook request
/// </summary>
public class PlisioWebhookRequest
{
    /// <summary>
    ///     Type of the operation (deposit, invoice, withdrawal, cash_out)
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    ///     Transaction ID
    /// </summary>
    public string TxnId { get; set; } = null!;

    /// <summary>
    ///     Status of the operation
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    ///     Amount of the operation
    /// </summary>
    public string Amount { get; set; } = null!;

    /// <summary>
    ///     Currency of the operation
    /// </summary>
    public string Currency { get; set; } = null!;
}