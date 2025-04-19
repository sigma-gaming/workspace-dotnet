using Core.Contracts.Enums;

namespace Core.Services.Payment.Interfaces;

/// <summary>
/// Base interface for all payment provider services
/// </summary>
public interface IPaymentProviderService
{
    /// <summary>
    /// Gets the payment provider type
    /// </summary>
    PaymentProvider Provider { get; }
    
    /// <summary>
    /// Gets the transaction status from the payment provider
    /// </summary>
    /// <param name="transactionId">Provider's transaction ID</param>
    /// <returns>Current payment status</returns>
    Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}
