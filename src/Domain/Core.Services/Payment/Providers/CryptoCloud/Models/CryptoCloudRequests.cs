using System.Text.Json.Serialization;

namespace Core.Services.Payment.Providers.CryptoCloud.Models;

/// <summary>
/// Request to create a new wallet
/// </summary>
public class CryptoCloudCreateWalletRequest
{
    /// <summary>
    /// Cryptocurrency code (BTC, ETH, TON, etc.)
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
    
    /// <summary>
    /// Callback URL for notifications
    /// </summary>
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }
    
    /// <summary>
    /// Optional label for the wallet
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }
}
