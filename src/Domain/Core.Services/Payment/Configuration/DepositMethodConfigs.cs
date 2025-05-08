using Core.Contracts.Enums;

namespace Core.Services.Payment.Configuration;

/// <summary>
///     Provides configuration for deposit methods
/// </summary>
public static class DepositMethodConfigs
{
    /// <summary>
    ///     Gets the list of deposit method configurations
    /// </summary>
    /// <returns>List of deposit method configurations</returns>
    public static List<DepositMethodConfig> GetConfigs()
    {
        return
        [
            // Plisio deposit methods
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Ton,
                Provider = PaymentProvider.Plisio,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
            },

            // CryptoCloud deposit methods
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Ton,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
            },
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Btc,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
            },
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Eth,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
            },
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.UsdtTrc20,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
            },
        ];
    }
}