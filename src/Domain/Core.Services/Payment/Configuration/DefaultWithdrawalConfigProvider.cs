using Core.Services.Payment.Interfaces;

namespace Core.Services.Payment.Configuration;

/// <summary>
/// Default implementation of IWithdrawalConfigProvider that returns production configs
/// </summary>
public class DefaultWithdrawalConfigProvider : IWithdrawalConfigProvider
{
    /// <summary>
    /// Gets all available withdrawal method configurations
    /// </summary>
    public List<WithdrawalMethodConfig> GetConfigs() => GetDefaultConfigs();
    
    /// <summary>
    /// Gets the default withdrawal configurations
    /// </summary>
    public static List<WithdrawalMethodConfig> GetDefaultConfigs() => WithdrawalMethodConfigs.GetConfigs();
} 