using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record GlobalTaskEntity
{
    public GlobalTaskKey Key { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public string Requirements { get; set; } = null!;

    public long Payout { get; set; }

    public int WageringMultiplier { get; set; }
}
