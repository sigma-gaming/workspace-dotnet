using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
/// Exception for errors related to payment providers
/// </summary>
public class PaymentProviderException : PaymentException
{
    public PaymentProvider Provider { get; }

    public PaymentProviderException(PaymentProvider provider, string message) 
        : base($"Provider {provider}: {message}")
    {
        Provider = provider;
    }

    public PaymentProviderException(PaymentProvider provider, string message, Exception innerException) 
        : base($"Provider {provider}: {message}", innerException)
    {
        Provider = provider;
    }
} 