using Core.Contracts.Enums;

namespace Core.Services.Payment.Models;

/// <summary>
/// Base class for payment responses
/// </summary>
public abstract class PaymentResponse
{
    /// <summary>
    /// Provider's transaction ID
    /// </summary>
    public required string ProviderTransactionId { get; init; }
    
    /// <summary>
    /// Current payment status
    /// </summary>
    public required PaymentStatus Status { get; init; }
    
    /// <summary>
    /// Amount in provider's currency
    /// </summary>
    public required string ProviderAmount { get; init; }
    
    /// <summary>
    /// Currency of the payment
    /// </summary>
    public required Currency Currency { get; init; }
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public required DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Last update timestamp
    /// </summary>
    public required DateTime UpdatedAt { get; init; }
}
