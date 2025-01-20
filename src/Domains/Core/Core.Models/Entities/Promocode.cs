using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record Promocode
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string Code { get; set; } = null!;

    public int WageringMultiplier { get; set; }

    public int Usages { get; set; }

    public int MaxUsages { get; set; }

    public bool IsActive { get; set; }

    public string Bonus { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string? Campaign { get; set; }

    public Guid? CreatedBy { get; set; }

    public PromocodeBonusType BonusType { get; set; }

    public User? CreatedByNavigation { get; set; }

    public ICollection<PromocodeUsage> PromocodeUsages { get; set; } = new List<PromocodeUsage>();

    public User? User { get; set; }
}
