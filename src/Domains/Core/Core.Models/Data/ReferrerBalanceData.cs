namespace Core.Models.Data;

public sealed record ReferrerBalanceData
{
    public required Guid ReferrerId { get; init; }
    public required long Available { get; init; }
} 