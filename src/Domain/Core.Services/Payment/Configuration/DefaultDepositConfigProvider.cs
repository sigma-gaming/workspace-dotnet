using Core.Services.Payment.Interfaces;

namespace Core.Services.Payment.Configuration;

/// <summary>
/// Default implementation of IDepositConfigProvider that returns production configs
/// </summary>
public class DefaultDepositConfigProvider : IDepositConfigProvider
{
    /// <summary>
    /// Gets all available deposit method configurations
    /// </summary>
    public List<DepositMethodConfig> GetConfigs() => GetDefaultConfigs();
    
    /// <summary>
    /// Gets the default deposit configurations
    /// </summary>
    public static List<DepositMethodConfig> GetDefaultConfigs() => DepositMethodConfigs.GetConfigs();
} 