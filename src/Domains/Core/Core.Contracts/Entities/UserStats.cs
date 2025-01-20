namespace Core.Contracts.Entities;

public sealed record UserStats
{
    public Guid UserId { get; set; }

    public int DepositCount { get; set; }

    public int WithdrawCount { get; set; }

    public User User { get; set; } = null!;
}
