namespace Core.Services.Payment.Configuration;

/// <summary>
///     Options for payment service configuration
/// </summary>
public class PaymentOptions
{
    /// <summary>
    ///     Deposit method configurations
    /// </summary>
    public List<DepositMethodConfig> DepositConfigs { get; set; } = [];

    /// <summary>
    ///     Withdrawal method configurations
    /// </summary>
    public List<WithdrawalMethodConfig> WithdrawalConfigs { get; set; } = [];
    
    /// <summary>
    ///     CryptoCloud provider configuration
    /// </summary>
    public ProviderConfig CryptoCloud { get; set; } = new();
    
    /// <summary>
    ///     Plisio provider configuration
    /// </summary>
    public ProviderConfig Plisio { get; set; } = new();
}