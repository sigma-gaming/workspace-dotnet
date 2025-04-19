using System.Security.Cryptography;
using System.Text;
using Core.Contracts.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
///     Handler for CryptoCloud webhooks
/// </summary>
public class CryptoCloudWebhookHandler
{
    private readonly ILogger<CryptoCloudWebhookHandler> logger;
    private readonly CryptoCloudOptions options;
    private readonly PaymentWebhookHandler webhookHandler;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CryptoCloudWebhookHandler" /> class
    /// </summary>
    public CryptoCloudWebhookHandler(
        IOptions<CryptoCloudOptions> options,
        PaymentWebhookHandler webhookHandler,
        ILogger<CryptoCloudWebhookHandler> logger)
    {
        this.options = options.Value;
        this.webhookHandler = webhookHandler;
        this.logger = logger;
    }

    /// <summary>
    ///     Handles a webhook notification from CryptoCloud
    /// </summary>
    public async Task<bool> HandleWebhookAsync(CryptoCloudWebhookRequest request)
    {
        try
        {
            // Verify the signature
            if (!VerifySignature(request))
            {
                logger.LogWarning("Invalid signature for CryptoCloud webhook");
                return false;
            }

            // Map the status
            var status = CryptoCloudStatusMapper.MapWalletStatus(request.Status);

            // Handle the webhook based on the type
            if (request.Type == "deposit")
                return await webhookHandler.HandleDepositWebhookAsync(
                    request.WalletId,
                    status,
                    PaymentProvider.CryptoCloud);

            logger.LogWarning("Unsupported webhook type: {Type}", request.Type);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle CryptoCloud webhook");
            return false;
        }
    }

    /// <summary>
    ///     Verifies the signature of a webhook request
    /// </summary>
    private bool VerifySignature(CryptoCloudWebhookRequest request)
    {
        try
        {
            // Create a string with all the request parameters
            var dataString = $"{request.WalletId}:{request.TxId}:{request.Amount}:{options.SecretKey}";

            // Calculate the hash
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataString));
            var calculatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            // Compare the signatures
            return string.Equals(calculatedSignature, request.Signature, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify CryptoCloud webhook signature");
            return false;
        }
    }
}

/// <summary>
///     CryptoCloud webhook request
/// </summary>
public class CryptoCloudWebhookRequest
{
    /// <summary>
    ///     Type of the operation (deposit)
    /// </summary>
    public string Type { get; set; } = "deposit";

    /// <summary>
    ///     Wallet ID
    /// </summary>
    public string WalletId { get; set; } = null!;

    /// <summary>
    ///     Transaction ID
    /// </summary>
    public string TxId { get; set; } = null!;

    /// <summary>
    ///     Amount of the operation
    /// </summary>
    public string Amount { get; set; } = null!;

    /// <summary>
    ///     Currency of the operation
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    ///     Status of the operation
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    ///     Signature of the request
    /// </summary>
    public string Signature { get; set; } = null!;
}