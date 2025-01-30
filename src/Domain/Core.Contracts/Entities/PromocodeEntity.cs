using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Contracts.Entities;

public sealed record PromocodeEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Code { get; set; } = null!;
    public int WageringMultiplier { get; set; }
    public int Usages { get; set; } = 0;
    public int MaxUsages { get; set; }
    public bool IsActive { get; set; } = true;
    public PromocodeBonus Bonus { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string? Campaign { get; set; }
    public Guid? CreatedBy { get; set; }
    public PromocodeBonusType BonusType { get; set; }
    public UserEntity? CreatedByNavigation { get; set; }
    public ICollection<PromocodeUsageEntity> PromocodeUsages { get; set; } = new List<PromocodeUsageEntity>();
    public UserEntity? User { get; set; }
}