using Core.Contracts.Enums;
using Core.Services.Payment.Configuration;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Test provider for deposit method configurations
/// </summary>
public static class TestDepositMethodConfigs
{
    /// <summary>
    ///     Get test configurations for deposit methods
    /// </summary>
    public static List<DepositMethodConfig> GetTestConfigs()
    {
        return
        [
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Ton,
                Provider = PaymentProvider.Plisio,
                MinAmount = 100,
                MaxAmount = 10000,
                CommissionRate = 100,
                Flow = DepositFlow.Redirect,
            },
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Btc,
                Provider = PaymentProvider.Plisio,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
                Flow = DepositFlow.Redirect,
            },
            new DepositMethodConfig
            {
                Method = DepositMethod.Crypto,
                Currency = Currency.Ton,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 100,
                Flow = DepositFlow.Redirect,
            },
        ];
    }
}