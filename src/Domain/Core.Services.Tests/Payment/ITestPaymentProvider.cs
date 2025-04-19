namespace Core.Services.Tests.Payment;

/// <summary>
/// Interface to mark and identify test payment providers
/// </summary>
public interface ITestPaymentProvider
{
    /// <summary>
    /// Unique identifier for this test provider implementation
    /// </summary>
    string TestIdentifier { get; }
} 