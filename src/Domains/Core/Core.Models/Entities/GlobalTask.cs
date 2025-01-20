using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record GlobalTask
{
    public GlobalTaskKey Key { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public string Requirements { get; set; } = null!;

    public long Payout { get; set; }

    public int WageringMultiplier { get; set; }
}
