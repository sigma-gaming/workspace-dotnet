using Core.Tests.Fixtures;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Cache fixture for payment service tests
/// </summary>
public class PaymentCacheFixture : CoreCacheFixture
{
    /// <summary>
    /// Create a new cache fixture with payment-specific configuration
    /// </summary>
    public PaymentCacheFixture() : base("payment_test")
    {
    }
} 