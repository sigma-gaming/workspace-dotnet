using Core.Tests.Fixtures;

namespace Core.Services.Tests.Payment;

/// <summary>
/// NATS fixture for payment service tests
/// </summary>
public class PaymentNatsFixture : CoreNatsFixture
{
    /// <summary>
    /// Create a new NATS fixture for payment service tests
    /// </summary>
    public PaymentNatsFixture() : base()
    {
    }
} 