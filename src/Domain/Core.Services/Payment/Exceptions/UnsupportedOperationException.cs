using Core.Contracts.Enums;

namespace Core.Services.Payment.Exceptions;

/// <summary>
/// Exception for unsupported payment operations
/// </summary>
public class UnsupportedOperationException : PaymentProviderException
{
    public string Operation { get; }

    public UnsupportedOperationException(PaymentProvider provider, string operation)
        : base(provider, $"Operation '{operation}' is not supported")
    {
        Operation = operation;
    }
} 