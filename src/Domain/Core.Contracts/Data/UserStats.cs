namespace Core.Contracts.Data;

public sealed record UserStats
{
    public required Guid UserId { get; init; }
    public required int DepositCount { get; init; }
    public required int WithdrawCount { get; init; }
} 