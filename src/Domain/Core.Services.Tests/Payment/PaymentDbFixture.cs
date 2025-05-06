using Core.Tests.Fixtures;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Database fixture for payment service tests
/// </summary>
public class PaymentDbFixture : CoreDbFixture
{
    /// <summary>
    ///     Create a new DB fixture with payment-specific configuration
    /// </summary>
    public PaymentDbFixture() : base("payment_test_db", "public", true)
    {
    }
}