namespace Core.Contracts.Data;

public sealed record ReferrerWithdrawal
{
    public required DateTime CreatedAt { get; init; }
    public required long Amount { get; init; }
    public Guid? ReferrerId { get; init; }
    public required Guid Id { get; init; }
}