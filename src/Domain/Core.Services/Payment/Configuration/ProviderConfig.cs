namespace Core.Services.Payment.Configuration;

/// <summary>
///     Configuration for payment providers
/// </summary>
public class ProviderConfig
{
    /// <summary>
    ///     API key for the provider
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    ///     Secret key for the provider (if required)
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    ///     Timeout in minutes for stale deposits
    /// </summary>
    public int StaleDepositTimeoutMinutes { get; set; } = 60; // Default 1 hour
    
    /// <summary>
    ///     Timeout in minutes for stale withdrawals 
    /// </summary>
    public int StaleWithdrawalTimeoutMinutes { get; set; } = 120; // Default 2 hours
}
