using Core.Services.Payment.Configuration;

namespace Core.Services.Payment.Interfaces;

/// <summary>
/// Provides withdrawal method configurations for payment processing
/// </summary>
public interface IWithdrawalConfigProvider
{
    /// <summary>
    /// Gets all available withdrawal method configurations
    /// </summary>
    List<WithdrawalMethodConfig> GetConfigs();
} 