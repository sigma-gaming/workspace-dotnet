using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
/// Exception for invalid payment amounts
/// </summary>
public class InvalidAmountException : PaymentException
{
    public decimal Amount { get; }
    public long? MinAmount { get; }
    public long? MaxAmount { get; }
    public Currency Currency { get; }

    public InvalidAmountException(
        decimal amount, 
        long? minAmount, 
        long? maxAmount, 
        Currency currency)
        : base($"Amount {amount} {currency} is outside the allowed range ({minAmount} - {maxAmount})")
    {
        Amount = amount;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Currency = currency;
    }
} 