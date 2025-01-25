namespace Core.Contracts.Entities;

public sealed record UserStatsEntity
{
    public Guid UserId { get; set; }

    public int DepositCount { get; set; }

    public int WithdrawCount { get; set; }

    public UserEntity User { get; set; } = null!;
}
