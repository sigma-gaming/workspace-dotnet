using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
///     Factory for creating payment-related exceptions
/// </summary>
public static class ExceptionFactory
{
    /// <summary>
    ///     Creates an exception for an unsupported deposit method
    /// </summary>
    public static UnsupportedPaymentMethodException UnsupportedDepositMethod(
        PaymentProvider provider,
        DepositMethod method,
        Currency currency)
    {
        return new UnsupportedPaymentMethodException(provider, method, currency);
    }

    /// <summary>
    ///     Creates an exception for an unsupported withdrawal method
    /// </summary>
    public static UnsupportedPaymentMethodException UnsupportedWithdrawalMethod(
        PaymentProvider provider,
        WithdrawalMethod method,
        Currency currency)
    {
        return new UnsupportedPaymentMethodException(provider, method, currency);
    }

    /// <summary>
    ///     Creates an exception for invalid payment amount
    /// </summary>
    public static InvalidAmountException InvalidAmount(
        decimal amount,
        long? minAmount,
        long? maxAmount,
        Currency currency)
    {
        return new InvalidAmountException(amount, minAmount, maxAmount, currency);
    }

    /// <summary>
    ///     Creates an exception for a payment provider error
    /// </summary>
    public static PaymentProviderException ProviderError(
        PaymentProvider provider,
        string message)
    {
        return new PaymentProviderException(provider, message);
    }

    /// <summary>
    ///     Creates an exception for a payment provider error with an inner exception
    /// </summary>
    public static PaymentProviderException ProviderError(
        PaymentProvider provider,
        string message,
        Exception innerException)
    {
        return new PaymentProviderException(provider, message, innerException);
    }

    /// <summary>
    ///     Creates an exception for an unsupported operation
    /// </summary>
    public static UnsupportedOperationException UnsupportedOperation(
        PaymentProvider provider,
        string operation)
    {
        return new UnsupportedOperationException(provider, operation);
    }

    /// <summary>
    ///     Creates an exception for a transaction not found
    /// </summary>
    public static TransactionNotFoundException TransactionNotFound(
        Guid id,
        string transactionType)
    {
        return new TransactionNotFoundException(id, transactionType);
    }

    /// <summary>
    ///     Creates an exception for a transaction not found by provider transaction ID
    /// </summary>
    public static TransactionNotFoundException TransactionNotFound(
        string providerTransactionId,
        string transactionType)
    {
        return new TransactionNotFoundException(providerTransactionId, transactionType);
    }

    /// <summary>
    ///     Creates an exception for a wallet-related error
    /// </summary>
    public static WalletException WalletError(
        Guid userId,
        Currency currency,
        PaymentProvider provider,
        string message)
    {
        return new WalletException(userId, currency, provider, message);
    }

    /// <summary>
    ///     Creates an exception for a wallet-related error with an inner exception
    /// </summary>
    public static WalletException WalletError(
        Guid userId,
        Currency currency,
        PaymentProvider provider,
        string message,
        Exception innerException)
    {
        return new WalletException(userId, currency, provider, message, innerException);
    }

    /// <summary>
    ///     Creates a generic payment exception
    /// </summary>
    public static PaymentException PaymentError(string message)
    {
        return new PaymentException(message);
    }

    /// <summary>
    ///     Creates a generic payment exception with an inner exception
    /// </summary>
    public static PaymentException PaymentError(string message, Exception innerException)
    {
        return new PaymentException(message, innerException);
    }

    /// <summary>
    ///     Creates an exception for an invalid payment operation
    /// </summary>
    public static InvalidOperationException InvalidOperation(string operationName, string message)
    {
        return new InvalidOperationException(operationName, message);
    }

    /// <summary>
    ///     Creates an exception for an invalid payment status transition
    /// </summary>
    public static InvalidOperationException InvalidStatusTransition(
        string transactionType,
        PaymentStatus currentStatus,
        string targetOperation)
    {
        return new InvalidOperationException(
            "StatusTransition",
            $"Cannot {targetOperation} {transactionType} with status {currentStatus}");
    }
}