using Core.Contracts.Enums;

namespace Core.Services.Payment.Configuration;

/// <summary>
///     Provides configuration for withdrawal methods
/// </summary>
public static class WithdrawalMethodConfigs
{
    /// <summary>
    ///     Gets the list of withdrawal method configurations
    /// </summary>
    /// <returns>List of withdrawal method configurations</returns>
    public static List<WithdrawalMethodConfig> GetConfigs()
    {
        return
        [
            new WithdrawalMethodConfig
            {
                Method = WithdrawalMethod.Crypto,
                Currency = Currency.Ton,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 1000,
                MaxAmount = 100000,
                CommissionRate = 200,
                ProcessingTimeHours = 24,
            },
        ];
    }
}