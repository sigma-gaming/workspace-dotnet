namespace Core.Contracts.Entities;

public sealed record BalanceEntity
{
    public Guid UserId { get; set; }

    public long Available { get; set; }

    public long WageringRequired { get; set; }

    public long TotalBet { get; set; }

    public long TotalWon { get; set; }

    public long TotalLost { get; set; }

    public int TotalBetCount { get; set; }

    public long MaxWin { get; set; }

    public int MaxMultiplier { get; set; }

    public Guid? MaxWinGameId { get; set; }

    public Guid? MaxMultiplierGameId { get; set; }

    public GameRecordEntity? MaxMultiplierGame { get; set; }

    public GameRecordEntity? MaxWinGame { get; set; }

    public UserEntity User { get; set; } = null!;
}
