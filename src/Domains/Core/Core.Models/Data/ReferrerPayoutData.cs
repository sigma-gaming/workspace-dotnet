namespace Core.Models.Data;

public sealed record ReferrerPayoutData
{
    public required Guid ReferrerId { get; init; }
    public required DateTime NextPayoutAt { get; init; }
    public DateTime? LastPayoutAt { get; init; }
} 