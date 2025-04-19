using Core.Contracts.Enums;

namespace Core.Services.Payment.Models;

/// <summary>
/// Base class for payment requests
/// </summary>
public abstract class PaymentRequest
{
    /// <summary>
    /// User ID making the payment
    /// </summary>
    public required Guid UserId { get; init; }
    
    /// <summary>
    /// User's IP address
    /// </summary>
    public required string UserIp { get; init; }
    
    /// <summary>
    /// Amount in gems (internal currency)
    /// </summary>
    public required long GemAmount { get; init; }
    
    /// <summary>
    /// Amount in the target currency
    /// </summary>
    public required decimal CurrencyAmount { get; init; }
    
    /// <summary>
    /// Payment provider to use
    /// </summary>
    public required PaymentProvider Provider { get; init; }
    
    /// <summary>
    /// Currency for the payment
    /// </summary>
    public required Currency Currency { get; init; }
}
