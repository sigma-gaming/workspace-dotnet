namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
/// Options for Plisio API
/// </summary>
public class PlisioOptions
{
    /// <summary>
    /// API key for Plisio
    /// </summary>
    public string ApiKey { get; set; } = null!;
    
    /// <summary>
    /// API URL for Plisio
    /// </summary>
    public string ApiUrl { get; set; } = "https://plisio.net/api/v1";
    
    /// <summary>
    /// Callback URL for Plisio to send notifications
    /// </summary>
    public string CallbackUrl { get; set; } = null!;
    
    /// <summary>
    /// Default currency for Plisio
    /// </summary>
    public string DefaultCurrency { get; set; } = "TON";
    
    /// <summary>
    /// Default fiat currency for Plisio
    /// </summary>
    public string DefaultFiatCurrency { get; set; } = "USD";
    
    /// <summary>
    /// Invoice expiration time in minutes
    /// </summary>
    public int InvoiceExpirationMinutes { get; set; } = 60;
}
