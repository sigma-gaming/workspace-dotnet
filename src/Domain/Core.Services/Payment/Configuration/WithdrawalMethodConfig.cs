using Core.Contracts.Enums;

namespace Core.Services.Payment.Configuration;

/// <summary>
/// Configuration for withdrawal methods
/// </summary>
public class WithdrawalMethodConfig : PaymentMethodConfig
{
    /// <summary>
    /// Withdrawal method
    /// </summary>
    public required WithdrawalMethod Method { get; init; }
    
    /// <summary>
    /// Processing time in hours (estimated)
    /// </summary>
    public int ProcessingTimeHours { get; init; } = 24;
}
