using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record Transaction
{
    public required DateTime CreatedAt { get; init; }
    public required long Amount { get; init; }
    public Guid? UserId { get; init; }
    public Guid? GameRecordId { get; init; }
    public required Guid Id { get; init; }
    public required TransactionType Type { get; init; }
    public Game? Game { get; init; }
}