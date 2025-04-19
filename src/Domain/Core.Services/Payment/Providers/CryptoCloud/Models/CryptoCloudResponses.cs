using System.Text.Json.Serialization;

namespace Core.Services.Payment.Providers.CryptoCloud.Models;

/// <summary>
/// Base response from CryptoCloud API
/// </summary>
public class CryptoCloudBaseResponse
{
    /// <summary>
    /// Status of the response (success or error)
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
    
    /// <summary>
    /// Error message if status is error
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>
/// Response for wallet creation
/// </summary>
public class CryptoCloudCreateWalletResponse : CryptoCloudBaseResponse
{
    /// <summary>
    /// Wallet data
    /// </summary>
    [JsonPropertyName("data")]
    public CryptoCloudWalletData? Data { get; set; }
}

/// <summary>
/// Wallet data
/// </summary>
public class CryptoCloudWalletData
{
    /// <summary>
    /// Wallet ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// Wallet address
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    
    /// <summary>
    /// Cryptocurrency code
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Wallet label
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    
    /// <summary>
    /// Wallet creation date
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = null!;
}

/// <summary>
/// Response for wallet list
/// </summary>
public class CryptoCloudWalletListResponse : CryptoCloudBaseResponse
{
    /// <summary>
    /// List of wallets
    /// </summary>
    [JsonPropertyName("data")]
    public List<CryptoCloudWalletData>? Data { get; set; }
}

/// <summary>
/// Response for wallet info
/// </summary>
public class CryptoCloudWalletInfoResponse : CryptoCloudBaseResponse
{
    /// <summary>
    /// Wallet info
    /// </summary>
    [JsonPropertyName("data")]
    public CryptoCloudWalletInfo? Data { get; set; }
}

/// <summary>
/// Wallet info
/// </summary>
public class CryptoCloudWalletInfo
{
    /// <summary>
    /// Wallet ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    /// <summary>
    /// Wallet address
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    
    /// <summary>
    /// Cryptocurrency code
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Wallet label
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    
    /// <summary>
    /// Wallet creation date
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = null!;
    
    /// <summary>
    /// Wallet balance
    /// </summary>
    [JsonPropertyName("balance")]
    public string Balance { get; set; } = null!;
    
    /// <summary>
    /// Wallet status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}
