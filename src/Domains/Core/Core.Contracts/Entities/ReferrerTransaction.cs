using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record ReferrerTransaction
{
    public DateTime CreatedAt { get; set; }
    public long Amount { get; set; }
    public Guid ReferrerId { get; set; }
    public bool IsProcessed { get; set; }
    public Guid? ReferralId { get; set; }
    public Guid? ReferralCampaignId { get; set; }
    public Guid Id { get; set; }
    public ReferralAction ReferralAction { get; set; }
    public User? Referral { get; set; }
    public ReferralCampaign? ReferralCampaign { get; set; }
    public User Referrer { get; set; } = null!;
}
