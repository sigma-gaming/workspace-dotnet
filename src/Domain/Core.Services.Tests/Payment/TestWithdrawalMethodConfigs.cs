using Core.Contracts.Enums;
using Core.Services.Payment.Configuration;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Test provider for withdrawal method configurations 
/// </summary>
public static class TestWithdrawalMethodConfigs
{
    /// <summary>
    /// Get test configurations for withdrawal methods
    /// </summary>
    public static List<WithdrawalMethodConfig> GetTestConfigs()
    {
        return 
        [
            new WithdrawalMethodConfig
            {
                Method = WithdrawalMethod.Plisio,
                Currency = Currency.Ton,
                Provider = PaymentProvider.Plisio,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 200,
                ProcessingTimeHours = 24,
            },
            new WithdrawalMethodConfig
            {
                Method = WithdrawalMethod.Plisio,
                Currency = Currency.Btc,
                Provider = PaymentProvider.Plisio,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 200,
                ProcessingTimeHours = 24,
            },
            new WithdrawalMethodConfig
            {
                Method = WithdrawalMethod.CryptoCloud,
                Currency = Currency.Ton,
                Provider = PaymentProvider.CryptoCloud,
                MinAmount = 100,
                MaxAmount = 1000000,
                CommissionRate = 200,
                ProcessingTimeHours = 24,
            }
        ];
    }
} 