using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record Withdrawal
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required long GemAmount { get; init; }
    public required decimal CurrencyAmount { get; init; }
    public required Guid UserId { get; init; }
    public Guid? TransactionId { get; init; }
    public required Guid Id { get; init; }
    public required string ProviderTransactionId { get; init; }
    public required string ProviderAmount { get; init; }
    public required PaymentStatus Status { get; init; }
    public required Currency Currency { get; init; }
    public required WithdrawalMethod Method { get; init; }
    public required PaymentProvider Provider { get; init; }
}