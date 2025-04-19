namespace Core.Services.Payment.Exceptions;

/// <summary>
/// Exception for invalid payment operations
/// </summary>
public class InvalidOperationException : PaymentException
{
    public string OperationName { get; }
    
    /// <summary>
    /// Creates a new instance of InvalidOperationException
    /// </summary>
    /// <param name="operationName">The name of the operation that failed</param>
    /// <param name="message">Additional details about why the operation is invalid</param>
    public InvalidOperationException(string operationName, string message)
        : base($"Invalid operation '{operationName}': {message}")
    {
        OperationName = operationName;
    }
} 