using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Services.Payment.Models;

/// <summary>
/// Response model for deposit creation
/// </summary>
public class DepositResponse : PaymentResponse
{
    /// <summary>
    /// Type of deposit
    /// </summary>
    public required DepositType Type { get; init; }
    
    /// <summary>
    /// Payload with deposit-specific data
    /// </summary>
    public required DepositPayload Payload { get; init; }
}
