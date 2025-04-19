using Core.Contracts.Enums;
using Core.Services.Payment.Models;

namespace Core.Services.Payment.Interfaces;

/// <summary>
///     Interface for deposit operations
/// </summary>
public interface IDepositProvider : IPaymentProviderService
{
    /// <summary>
    ///     Creates a deposit request with the payment provider
    /// </summary>
    /// <param name="input">Deposit request details</param>
    /// <returns>Deposit response with payment details</returns>
    Task<DepositResponse> CreateDepositAsync(DepositInput input);

    /// <summary>
    ///     Gets the status of a deposit from the payment provider
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    /// <returns>Current deposit status</returns>
    Task<PaymentStatus> GetDepositStatusAsync(string providerTransactionId);

    /// <summary>
    ///     Cancels a deposit with the payment provider
    /// </summary>
    /// <param name="providerTransactionId">Provider's transaction ID</param>
    Task CancelDepositAsync(string providerTransactionId);
}