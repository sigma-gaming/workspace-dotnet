using Core.Contracts.Enums;

namespace Core.Services.Payment.Models;

/// <summary>
///     Request model for creating a withdrawal
/// </summary>
public record WithdrawalInput(
    Guid UserId,
    long GemAmount,
    WithdrawalMethod Method,
    Currency Currency,
    PaymentProvider Provider,
    string DestinationAddress
);