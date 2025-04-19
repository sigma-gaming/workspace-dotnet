using Core.Services.Payment.Interfaces;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Configuration;

/// <summary>
/// Provides withdrawal method configurations from IOptions
/// </summary>
public class OptionsBasedWithdrawalConfigProvider(IOptions<PaymentOptions> options) : IWithdrawalConfigProvider
{
    private readonly PaymentOptions _options = options.Value;

    /// <summary>
    /// Gets all available withdrawal method configurations from options
    /// </summary>
    public List<WithdrawalMethodConfig> GetConfigs() => _options.WithdrawalConfigs;
} 