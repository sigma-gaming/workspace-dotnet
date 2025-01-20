using Core.Domain.Enums;

namespace Core.Models.Data;

public sealed record SessionData
{
    public required DateTime ExpiresAt { get; init; }
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public Guid? ReferrerId { get; init; }
    public Guid? ReferralCampaignId { get; init; }
    public required AccountProvider Provider { get; init; }
} 