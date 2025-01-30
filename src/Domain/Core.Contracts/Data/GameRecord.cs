using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Contracts.Data;

public sealed record GameRecord
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Game Game { get; init; }
    public required GameOutcome Outcome { get; init; }
    public required GameSnapshot Snapshot { get; init; }
    public required long Bet { get; init; }
    public required int Multiplier { get; init; }
    public required long Payout { get; init; }
    public required string PreviewUserName { get; init; }
    public required Guid UserId { get; init; }
    public Guid? TransactionId { get; init; }
} 