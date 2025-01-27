namespace Core.Contracts.Entities;

public sealed record BalanceEntity
{
    public Guid UserId { get; set; }
    public long Available { get; set; } = 0;
    public long WageringRequired { get; set; } = 0;
    public long TotalBet { get; set; } = 0;
    public long TotalWon { get; set; } = 0;
    public long TotalLost { get; set; } = 0;
    public int TotalBetCount { get; set; } = 0;
    public long MaxWin { get; set; } = 0;
    public int MaxMultiplier { get; set; } = 0;
    public Guid? MaxWinGameId { get; set; }
    public Guid? MaxMultiplierGameId { get; set; }
    public GameRecordEntity? MaxMultiplierGame { get; set; }
    public GameRecordEntity? MaxWinGame { get; set; }
    public UserEntity User { get; set; } = null!;
}
