namespace Core.Contracts.Entities;

public sealed record UserStatsEntity
{
    public Guid UserId { get; set; }
    public int DepositCount { get; set; } = 0;
    public int WithdrawCount { get; set; } = 0;
    public UserEntity User { get; set; } = null!;
}
