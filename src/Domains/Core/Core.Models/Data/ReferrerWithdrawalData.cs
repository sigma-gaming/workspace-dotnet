namespace Core.Models.Data;

public sealed record ReferrerWithdrawalData
{
    public required DateTime CreatedAt { get; init; }
    public required long Amount { get; init; }
    public Guid? ReferrerId { get; init; }
    public required Guid Id { get; init; }
} 