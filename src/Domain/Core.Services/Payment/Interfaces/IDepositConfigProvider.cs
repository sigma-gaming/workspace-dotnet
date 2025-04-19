using Core.Services.Payment.Configuration;

namespace Core.Services.Payment.Interfaces;

/// <summary>
/// Provides deposit method configurations for payment processing
/// </summary>
public interface IDepositConfigProvider
{
    /// <summary>
    /// Gets all available deposit method configurations
    /// </summary>
    List<DepositMethodConfig> GetConfigs();
} 