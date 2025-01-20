using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record PromocodeUsage
{
    public DateTime CreatedAt { get; set; }

    public Guid PromocodeId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public PromocodeUsageStatus Status { get; set; }

    public Promocode Promocode { get; set; } = null!;

    public User User { get; set; } = null!;
}
