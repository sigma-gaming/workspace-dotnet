using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record PromocodeUsageEntity
{
    public DateTime CreatedAt { get; set; }

    public Guid PromocodeId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public PromocodeUsageStatus Status { get; set; }

    public PromocodeEntity Promocode { get; set; } = null!;

    public UserEntity User { get; set; } = null!;
}
