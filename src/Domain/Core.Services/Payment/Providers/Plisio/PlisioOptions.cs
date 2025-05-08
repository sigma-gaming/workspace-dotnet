namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Options for Plisio API
/// </summary>
public class PlisioOptions
{
    /// <summary>
    ///     API key for Plisio
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    ///     Invoice expiration time in minutes
    /// </summary>
    public int InvoiceExpirationMinutes { get; set; } = 60;
}