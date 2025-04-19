using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
///     Exception for wallet-related errors
/// </summary>
public class WalletException : PaymentException
{
    public WalletException(string message)
        : base(message)
    {
    }

    public WalletException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public WalletException(Guid userId, Currency currency, PaymentProvider provider, string message)
        : base($"Wallet error for user {userId}, currency {currency}, provider {provider}: {message}")
    {
        UserId = userId;
        Currency = currency;
        Provider = provider;
    }

    public WalletException(Guid userId, Currency currency, PaymentProvider provider, string message,
        Exception innerException)
        : base($"Wallet error for user {userId}, currency {currency}, provider {provider}: {message}", innerException)
    {
        UserId = userId;
        Currency = currency;
        Provider = provider;
    }

    public Guid? UserId { get; }
    public Currency? Currency { get; }
    public PaymentProvider? Provider { get; }
}