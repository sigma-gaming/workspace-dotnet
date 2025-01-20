using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record GlobalTaskData
{
    public required GlobalTaskKey Key { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsActive { get; init; }
    public required string Requirements { get; init; }
    public required long Payout { get; init; }
    public required int WageringMultiplier { get; init; }
} 