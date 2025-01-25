using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record GameRecord
{
    public required Game Game { get; init; }
    public required GameOutcome Outcome { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Snapshot { get; init; }
    public required long Bet { get; init; }
    public required int Multiplier { get; init; }
    public required long Payout { get; init; }
    public required string PreviewUserName { get; init; }
    public required Guid UserId { get; init; }
    public Guid? TransactionId { get; init; }
    public required Guid Id { get; init; }
} 