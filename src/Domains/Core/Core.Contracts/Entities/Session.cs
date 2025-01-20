using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record Session
{
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
    public Guid? ReferrerId { get; set; }
    public Guid? ReferralCampaignId { get; set; }
    public AccountProvider Provider { get; set; }
    public ReferralCampaign? ReferralCampaign { get; set; }
    public User? Referrer { get; set; }
    public User User { get; set; } = null!;
}
