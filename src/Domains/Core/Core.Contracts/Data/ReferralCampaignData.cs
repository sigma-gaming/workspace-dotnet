namespace Core.Contracts.Data;

public sealed record ReferralCampaignData
{
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required int TotalVisits { get; init; }
    public required int TotalSignups { get; init; }
    public Guid? ReferrerId { get; init; }
    public required Guid Id { get; init; }
} 