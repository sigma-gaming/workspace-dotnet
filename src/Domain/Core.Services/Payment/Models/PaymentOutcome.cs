namespace Core.Services.Payment.Models;

/// <summary>
/// Possible outcomes of payment operations
/// </summary>
public enum PaymentOutcome
{
    /// <summary>
    /// Operation completed successfully
    /// </summary>
    Success,
    
    /// <summary>
    /// Insufficient funds for the operation
    /// </summary>
    InsufficientFunds,
    
    /// <summary>
    /// Invalid amount (too small or too large)
    /// </summary>
    InvalidAmount,
    
    /// <summary>
    /// Unsupported payment method
    /// </summary>
    UnsupportedMethod,
    
    /// <summary>
    /// Unsupported currency
    /// </summary>
    UnsupportedCurrency,
    
    /// <summary>
    /// Error from the payment provider
    /// </summary>
    ProviderError,
    
    /// <summary>
    /// Operation failed for other reasons
    /// </summary>
    Failed
}
