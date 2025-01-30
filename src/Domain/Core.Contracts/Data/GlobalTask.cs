using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Contracts.Data;

public sealed record GlobalTask
{
    public required GlobalTaskKey Key { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsActive { get; init; }
    public required GlobalTaskRequirements Requirements { get; init; }
    public required long Payout { get; init; }
    public required int WageringMultiplier { get; init; }
}