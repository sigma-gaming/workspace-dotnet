using Core.Contracts.Enums;
using Core.Services.Payment.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment.Providers;

/// <summary>
///     Base class for payment provider services
/// </summary>
public abstract class PaymentProviderService(ILogger logger) : IPaymentProviderService
{
    protected readonly ILogger Logger = logger;

    /// <summary>
    ///     Gets the payment provider type
    /// </summary>
    public abstract PaymentProvider Provider { get; }

    /// <summary>
    ///     Gets the transaction status from the payment provider
    /// </summary>
    /// <param name="transactionId">Provider's transaction ID</param>
    /// <returns>Current payment status</returns>
    public abstract Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}