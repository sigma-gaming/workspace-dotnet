using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record ReferrerTransaction
{
    public required DateTime CreatedAt { get; init; }
    public required long Amount { get; init; }
    public required Guid ReferrerId { get; init; }
    public required bool IsProcessed { get; init; }
    public Guid? ReferralId { get; init; }
    public Guid? ReferralCampaignId { get; init; }
    public required Guid Id { get; init; }
    public required ReferralAction ReferralAction { get; init; }
    public required string ReferralName { get; init; }
    public string? ReferralImage { get; init; }
    public string? ReferralUsername { get; init; }
}