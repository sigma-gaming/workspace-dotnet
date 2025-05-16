using System.Text.Json.Serialization;
using Core.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
///     Handler for CryptoCloud webhooks
/// </summary>
public class CryptoCloudWebhookHandler
{
    private readonly ILogger<CryptoCloudWebhookHandler> logger;
    private readonly PaymentWebhookHandler webhookHandler;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CryptoCloudWebhookHandler" /> class
    /// </summary>
    public CryptoCloudWebhookHandler(
        PaymentWebhookHandler webhookHandler,
        ILogger<CryptoCloudWebhookHandler> logger)
    {
        this.webhookHandler = webhookHandler;
        this.logger = logger;
    }

    /// <summary>
    ///     Handles a webhook notification from CryptoCloud, the token and the status are verified by
    ///     <see cref="CryptoCloudWebhookVerifier" />
    /// </summary>
    public async Task<bool> HandleWebhookAsync(CryptoCloudWebhookRequest request)
    {
        try
        {
            // Map the payment status
            var paymentStatus = PaymentStatus.Completed;

            // Create a unique identifier for the transaction
            var transactionId = request.InvoiceId;

            // Get additional information from invoice_info if present
            var currency = request.Currency;
            var amount = request.AmountCrypto;

            logger.LogInformation(
                "Processing CryptoCloud payment webhook: Invoice={InvoiceId}, Amount={Amount}, Currency={Currency}",
                transactionId, amount, currency);

            // Handle deposit transaction update
            return await webhookHandler.HandleDepositWebhookAsync(
                transactionId,
                paymentStatus,
                PaymentProvider.CryptoCloud);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle CryptoCloud webhook for invoice {InvoiceId}", request.InvoiceId);
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
    ///     Status of the webhook request
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    /// <summary>
    ///     Invoice ID
    /// </summary>
    [JsonPropertyName("invoice_id")]
    public string InvoiceId { get; set; } = null!;

    /// <summary>
    ///     Amount of the operation in cryptocurrency
    /// </summary>
    [JsonPropertyName("amount_crypto")]
    public decimal AmountCrypto { get; set; }

    /// <summary>
    ///     Currency code (e.g., BTC, USDT_TRC20)
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    /// <summary>
    ///     Order ID from the merchant system
    /// </summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = null!;

    /// <summary>
    ///     JWT token for signature verification
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    /// <summary>
    ///     Detailed invoice information
    /// </summary>
    [JsonPropertyName("invoice_info")]
    public CryptoCloudInvoiceInfo? InvoiceInfo { get; set; }
}

/// <summary>
///     CryptoCloud invoice information
/// </summary>
public class CryptoCloudInvoiceInfo
{
    /// <summary>
    ///     UUID of the invoice
    /// </summary>
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = null!;

    /// <summary>
    ///     Creation date and time
    /// </summary>
    [JsonPropertyName("created")]
    public string Created { get; set; } = null!;

    /// <summary>
    ///     Payment address
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;

    /// <summary>
    ///     Detailed currency information
    /// </summary>
    [JsonPropertyName("currency")]
    public CryptoCloudCurrencyInfo Currency { get; set; } = null!;

    /// <summary>
    ///     Date when payment was completed
    /// </summary>
    [JsonPropertyName("date_finished")]
    public string DateFinished { get; set; } = null!;

    /// <summary>
    ///     Expiry date of the invoice
    /// </summary>
    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; } = null!;

    /// <summary>
    ///     Status of the invoice
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    /// <summary>
    ///     Detailed invoice status
    /// </summary>
    [JsonPropertyName("invoice_status")]
    public string InvoiceStatus { get; set; } = null!;

    /// <summary>
    ///     Amount to be paid
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Amount actually paid
    /// </summary>
    [JsonPropertyName("amount_paid")]
    public decimal AmountPaid { get; set; }

    /// <summary>
    ///     List of transaction IDs
    /// </summary>
    [JsonPropertyName("tx_list")]
    public List<string> TxList { get; set; } = new();
}

/// <summary>
///     CryptoCloud currency information
/// </summary>
public class CryptoCloudCurrencyInfo
{
    /// <summary>
    ///     Currency ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     Currency code
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    /// <summary>
    ///     Full currency code including network
    /// </summary>
    [JsonPropertyName("fullcode")]
    public string FullCode { get; set; } = null!;

    /// <summary>
    ///     Network information
    /// </summary>
    [JsonPropertyName("network")]
    public CryptoCloudNetworkInfo Network { get; set; } = null!;
}

/// <summary>
///     CryptoCloud network information
/// </summary>
public class CryptoCloudNetworkInfo
{
    /// <summary>
    ///     Network ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     Network code
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    /// <summary>
    ///     Full network name
    /// </summary>
    [JsonPropertyName("fullname")]
    public string FullName { get; set; } = null!;
}