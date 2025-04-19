using Core.Contracts.Entities;
using Core.Contracts.Enums;

namespace Core.Services.Payment.Models;

/// <summary>
/// Output model for withdrawal operations
/// </summary>
public class WithdrawalOutput
{
    /// <summary>
    /// Outcome of the withdrawal operation
    /// </summary>
    public required PaymentOutcome Outcome { get; init; }
    
    /// <summary>
    /// Withdrawal entity (only for successful operations)
    /// </summary>
    public WithdrawalEntity? Withdrawal { get; init; }
    
    /// <summary>
    /// Updated balance (only for successful operations)
    /// </summary>
    public long? UpdatedBalance { get; init; }
    
    /// <summary>
    /// Available balance (for InsufficientFunds outcome)
    /// </summary>
    public long? Available { get; init; }
    
    /// <summary>
    /// Minimum amount (for InvalidAmount outcome)
    /// </summary>
    public long? MinAmount { get; init; }
    
    /// <summary>
    /// Maximum amount (for InvalidAmount outcome)
    /// </summary>
    public long? MaxAmount { get; init; }
    
    /// <summary>
    /// Currency (for InvalidAmount or UnsupportedCurrency outcomes)
    /// </summary>
    public Currency? Currency { get; init; }
    
    /// <summary>
    /// Method (for UnsupportedMethod outcome)
    /// </summary>
    public WithdrawalMethod? Method { get; init; }
    
    /// <summary>
    /// Provider (for UnsupportedMethod outcome)
    /// </summary>
    public PaymentProvider? Provider { get; init; }
    
    /// <summary>
    /// Error message (for ProviderError or Failed outcomes)
    /// </summary>
    public string? Error { get; init; }
}
