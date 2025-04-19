using System.Text.Json.Serialization;

namespace Core.Services.Payment.Providers.Plisio.Models;

/// <summary>
/// Base response from Plisio API
/// </summary>
public class PlisioBaseResponse
{
    /// <summary>
    /// Status of the response (success or error)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}

/// <summary>
/// Success response from Plisio API
/// </summary>
/// <typeparam name="T">Type of the data</typeparam>
public class PlisioSuccessResponse<T> : PlisioBaseResponse
{
    /// <summary>
    /// Response data
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}

/// <summary>
/// Error response from Plisio API
/// </summary>
public class PlisioErrorResponse : PlisioBaseResponse
{
    /// <summary>
    /// Error data
    /// </summary>
    [JsonPropertyName("data")]
    public PlisioError Data { get; set; } = null!;
}

/// <summary>
/// Error details
/// </summary>
public class PlisioError
{
    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
    
    /// <summary>
    /// Error code
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }
}

/// <summary>
/// Invoice response data
/// </summary>
public class PlisioInvoiceResponse
{
    /// <summary>
    /// Plisio's internal ID
    /// </summary>
    [JsonPropertyName("txn_id")]
    public string TransactionId { get; set; } = null!;
    
    /// <summary>
    /// Invoice URL
    /// </summary>
    [JsonPropertyName("invoice_url")]
    public string InvoiceUrl { get; set; } = null!;
    
    /// <summary>
    /// Total sum of the invoice
    /// </summary>
    [JsonPropertyName("invoice_total_sum")]
    public string InvoiceTotalSum { get; set; } = null!;
    
    /// <summary>
    /// Plisio's internal ID (copy of txn_id)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// Invoice amount in the selected cryptocurrency
    /// </summary>
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = null!;
    
    /// <summary>
    /// Invoice hash
    /// </summary>
    [JsonPropertyName("wallet_hash")]
    public string WalletHash { get; set; } = null!;
    
    /// <summary>
    /// ID column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("psys_cid")]
    public string PsysCid { get; set; } = null!;
    
    /// <summary>
    /// Code column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Fiat currency
    /// </summary>
    [JsonPropertyName("source_currency")]
    public string SourceCurrency { get; set; } = null!;
    
    /// <summary>
    /// Exchange rate
    /// </summary>
    [JsonPropertyName("source_rate")]
    public string SourceRate { get; set; } = null!;
    
    /// <summary>
    /// Status of the invoice
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    
    /// <summary>
    /// Invoice read-only view key
    /// </summary>
    [JsonPropertyName("view_key")]
    public string ViewKey { get; set; } = null!;
}

/// <summary>
/// Invoice details response
/// </summary>
public class PlisioInvoiceDetailsResponse
{
    /// <summary>
    /// Invoice data
    /// </summary>
    [JsonPropertyName("invoice")]
    public PlisioInvoiceResponse Invoice { get; set; } = null!;
}

/// <summary>
/// Withdrawal response data
/// </summary>
public class PlisioWithdrawalResponse
{
    /// <summary>
    /// Type of withdrawal
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    
    /// <summary>
    /// Status of the withdrawal
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    
    /// <summary>
    /// ID column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("psys_cid")]
    public string PsysCid { get; set; } = null!;
    
    /// <summary>
    /// Code column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Transaction fee
    /// </summary>
    [JsonPropertyName("fee")]
    public string Fee { get; set; } = null!;
    
    /// <summary>
    /// Destination hash
    /// </summary>
    [JsonPropertyName("wallet_hash")]
    public string WalletHash { get; set; } = null!;
    
    /// <summary>
    /// Transfer amount in cryptocurrency
    /// </summary>
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = null!;
    
    /// <summary>
    /// Link to the cryptocurrency block explorer
    /// </summary>
    [JsonPropertyName("tx_url")]
    public string TxUrl { get; set; } = null!;
    
    /// <summary>
    /// Array of transaction ids
    /// </summary>
    [JsonPropertyName("tx_id")]
    public List<string> TxId { get; set; } = new List<string>();
    
    /// <summary>
    /// Internal Plisio operation ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// Internal Plisio operation ID
    /// </summary>
    [JsonPropertyName("txn_id")]
    public string TransactionId { get; set; } = null!;
}

/// <summary>
/// Operation response data
/// </summary>
public class PlisioOperationResponse
{
    /// <summary>
    /// Status of the operation
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    
    /// <summary>
    /// ID column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("psys_cid")]
    public string PsysCid { get; set; } = null!;
    
    /// <summary>
    /// Code column from supported cryptocurrencies
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Internal Plisio operation ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// Array of transaction ids
    /// </summary>
    [JsonPropertyName("tx_id")]
    public List<string> TxId { get; set; } = new List<string>();
}

/// <summary>
/// Cryptocurrency response data
/// </summary>
public class PlisioCryptocurrencyResponse
{
    /// <summary>
    /// Cryptocurrency name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Cryptocurrency Plisio Abbr
    /// </summary>
    [JsonPropertyName("cid")]
    public string Cid { get; set; } = null!;
    
    /// <summary>
    /// Cryptocurrency Abbr
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Cryptocurrency USD rate
    /// </summary>
    [JsonPropertyName("rate_usd")]
    public string RateUsd { get; set; } = null!;
}
