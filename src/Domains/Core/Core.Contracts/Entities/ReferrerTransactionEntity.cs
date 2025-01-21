using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record ReferrerTransactionEntity
{
    public DateTime CreatedAt { get; set; }
    public long Amount { get; set; }
    public Guid ReferrerId { get; set; }
    public bool IsProcessed { get; set; }
    public Guid? ReferralId { get; set; }
    public Guid? ReferralCampaignId { get; set; }
    public Guid Id { get; set; }
    public ReferralAction ReferralAction { get; set; }
    public UserEntity? Referral { get; set; }
    public ReferralCampaignEntity? ReferralCampaign { get; set; }
    public UserEntity Referrer { get; set; } = null!;
}
