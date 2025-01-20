using Core.Domain.Enums;

namespace Core.Models.Data;

public sealed record ReferrerTransactionData
{
    public required DateTime CreatedAt { get; init; }
    public required long Amount { get; init; }
    public required Guid ReferrerId { get; init; }
    public required bool IsProcessed { get; init; }
    public Guid? ReferralId { get; init; }
    public Guid? ReferralCampaignId { get; init; }
    public required Guid Id { get; init; }
    public required ReferralAction ReferralAction { get; init; }
} 