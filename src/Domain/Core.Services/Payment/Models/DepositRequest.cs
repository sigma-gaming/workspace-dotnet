using Core.Contracts.Enums;

namespace Core.Services.Payment.Models;

/// <summary>
///     Request model for creating a deposit
/// </summary>
public record DepositInput(
    Guid UserId,
    long GemAmount,
    DepositMethod Method,
    Currency Currency,
    PaymentProvider Provider
);