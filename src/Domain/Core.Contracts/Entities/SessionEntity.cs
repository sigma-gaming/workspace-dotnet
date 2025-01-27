using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record SessionEntity
{
    public Guid Id { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public Guid? ReferrerId { get; set; }
    public Guid? ReferralCampaignId { get; set; }
    public AccountProvider Provider { get; set; }
    public ReferralCampaignEntity? ReferralCampaign { get; set; }
    public UserEntity? Referrer { get; set; }
    public UserEntity User { get; set; } = null!;
}
