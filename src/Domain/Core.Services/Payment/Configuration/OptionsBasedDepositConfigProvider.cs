using Core.Services.Payment.Interfaces;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Configuration;

/// <summary>
/// Provides deposit method configurations from IOptions
/// </summary>
public class OptionsBasedDepositConfigProvider(IOptions<PaymentOptions> options) : IDepositConfigProvider
{
    private readonly PaymentOptions _options = options.Value;

    /// <summary>
    /// Gets all available deposit method configurations from options
    /// </summary>
    public List<DepositMethodConfig> GetConfigs() => _options.DepositConfigs;
} 