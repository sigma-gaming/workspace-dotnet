using System.Security.Cryptography;
using System.Text;
using Core.Contracts.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Handler for Plisio webhooks
/// </summary>
public class PlisioWebhookHandler(
    PaymentWebhookHandler webhookHandler,
    IOptions<PlisioOptions> options,
    ILogger<PlisioWebhookHandler> logger)
{
    private readonly PlisioOptions options = options.Value;

    /// <summary>
    ///     Handles a webhook notification from Plisio
    /// </summary>
    public async Task<bool> HandleWebhookAsync(PlisioWebhookRequest request, string signature)
    {
        try
        {
            // Verify the signature
            if (!VerifySignature(request, signature))
            {
                logger.LogWarning("Invalid signature for Plisio webhook");
                return false;
            }

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

    /// <summary>
    ///     Verifies the signature of a webhook request
    /// </summary>
    private bool VerifySignature(PlisioWebhookRequest request, string signature)
    {
        try
        {
            // Create a string with all the request parameters
            var dataString = $"{request.TxnId}:{request.Status}:{request.Amount}:{options.ApiKey}";

            // Calculate the hash
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataString));
            var calculatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            // Compare the signatures
            return string.Equals(calculatedSignature, signature, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify Plisio webhook signature");
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