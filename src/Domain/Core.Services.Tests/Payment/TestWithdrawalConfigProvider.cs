using Core.Services.Payment.Configuration;
using Core.Services.Payment.Interfaces;

namespace Core.Services.Tests.Payment;

public class TestWithdrawalConfigProvider : IWithdrawalConfigProvider
{
    public List<WithdrawalMethodConfig> GetConfigs()
    {
        return TestWithdrawalMethodConfigs.GetTestConfigs();
    }
}