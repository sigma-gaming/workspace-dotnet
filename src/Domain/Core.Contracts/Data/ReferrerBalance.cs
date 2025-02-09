namespace Core.Contracts.Data;

public sealed record ReferrerBalance
{
    public required Guid ReferrerId { get; init; }
    public required long Available { get; init; }
}