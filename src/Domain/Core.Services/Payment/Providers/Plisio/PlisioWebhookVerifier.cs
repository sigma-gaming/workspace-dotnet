using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Verifies Plisio webhook signatures before the webhook is queued
/// </summary>
public class PlisioWebhookVerifier(
    IOptions<PlisioOptions> options,
    ILogger<PlisioWebhookVerifier> logger)
{
    private readonly PlisioOptions options = options.Value;

    /// <summary>
    ///     Verifies the signature of a webhook request
    /// </summary>
    public bool VerifySignature(PlisioWebhookRequest request, string signature)
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