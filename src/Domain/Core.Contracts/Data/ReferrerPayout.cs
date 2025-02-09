namespace Core.Contracts.Data;

public sealed record ReferrerPayout
{
    public required Guid ReferrerId { get; init; }
    public required DateTime NextPayoutAt { get; init; }
    public DateTime? LastPayoutAt { get; init; }
}