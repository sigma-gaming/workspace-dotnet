namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
/// Options for CryptoCloud API
/// </summary>
public class CryptoCloudOptions
{
    /// <summary>
    /// API key for CryptoCloud
    /// </summary>
    public string ApiKey { get; set; } = null!;
    
    /// <summary>
    /// Secret key for CryptoCloud webhook signature verification
    /// </summary>
    public string SecretKey { get; set; } = null!;
    
    /// <summary>
    /// API URL for CryptoCloud
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.cryptocloud.plus";
    
    /// <summary>
    /// Callback URL for CryptoCloud to send notifications
    /// </summary>
    public string CallbackUrl { get; set; } = null!;
    
    /// <summary>
    /// Whether to use sandbox environment
    /// </summary>
    public bool UseSandbox { get; set; } = false;
}
