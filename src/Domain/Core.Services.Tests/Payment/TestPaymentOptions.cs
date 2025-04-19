using Core.Services.Payment.Configuration;
using Microsoft.Extensions.Options;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Provides test payment options for integration tests
/// </summary>
public class TestPaymentOptions : IOptions<PaymentOptions>
{
    public PaymentOptions Value { get; } = new()
    {
        DepositConfigs = TestDepositMethodConfigs.GetTestConfigs(),
        WithdrawalConfigs = TestWithdrawalMethodConfigs.GetTestConfigs(),
    };
}