using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
/// Exception for unsupported payment methods
/// </summary>
public class UnsupportedPaymentMethodException : PaymentProviderException
{
    public DepositMethod? DepositMethod { get; }
    public WithdrawalMethod? WithdrawalMethod { get; }
    public Currency Currency { get; }

    public UnsupportedPaymentMethodException(PaymentProvider provider, DepositMethod method, Currency currency)
        : base(provider, $"Method {method} with currency {currency} is not supported")
    {
        DepositMethod = method;
        Currency = currency;
    }
    
    public UnsupportedPaymentMethodException(PaymentProvider provider, WithdrawalMethod method, Currency currency)
        : base(provider, $"Method {method} with currency {currency} is not supported")
    {
        WithdrawalMethod = method;
        Currency = currency;
    }
} 