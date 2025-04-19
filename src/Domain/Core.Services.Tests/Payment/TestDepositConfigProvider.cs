using Core.Services.Payment.Configuration;
using Core.Services.Payment.Interfaces;

namespace Core.Services.Tests.Payment;

public class TestDepositConfigProvider : IDepositConfigProvider
{
    public List<DepositMethodConfig> GetConfigs() => TestDepositMethodConfigs.GetTestConfigs();
} 