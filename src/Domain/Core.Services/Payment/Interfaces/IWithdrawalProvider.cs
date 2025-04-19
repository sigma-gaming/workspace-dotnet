using Core.Contracts.Enums;
using Core.Services.Payment.Models;

namespace Core.Services.Payment.Interfaces;

/// <summary>
///     Interface for withdrawal operations
/// </summary>
public interface IWithdrawalProvider : IPaymentProviderService
{
    /// <summary>
    ///     Creates a withdrawal request with the payment provider
    /// </summary>
    /// <param name="input">Withdrawal request details</param>
    /// <returns>Withdrawal response with payment details</returns>
    Task<WithdrawalResponse> CreateWithdrawalAsync(WithdrawalInput input);

    /// <summary>
    ///     Gets the status of a withdrawal from the payment provider
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    /// <returns>Current withdrawal status</returns>
    Task<PaymentStatus> GetWithdrawalStatusAsync(string providerTransactionId);

    /// <summary>
    ///     Cancels a withdrawal with the payment provider
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    Task CancelWithdrawalAsync(string providerTransactionId);
}