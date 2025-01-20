namespace Core.Models.Data;

public sealed record UserStatsData
{
    public required Guid UserId { get; init; }
    public required int DepositCount { get; init; }
    public required int WithdrawCount { get; init; }
} 