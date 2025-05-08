namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
///     Options for CryptoCloud API
/// </summary>
public class CryptoCloudOptions
{
    /// <summary>
    ///     API key for CryptoCloud
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    ///     Secret key for CryptoCloud webhook signature verification
    /// </summary>
    public string SecretKey { get; set; } = null!;
}