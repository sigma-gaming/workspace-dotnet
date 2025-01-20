namespace Core.Contracts.Data;

public sealed record BalanceData
{
    public required Guid UserId { get; init; }
    public required long Available { get; init; }
    public required long WageringRequired { get; init; }
    public required long TotalBet { get; init; }
    public required long TotalWin { get; init; }
    public required long TotalLost { get; init; }
    public required int TotalBetCount { get; init; }
    public required long MaxWin { get; init; }
    public required int MaxMultiplier { get; init; }
    public Guid? MaxWinGameId { get; init; }
    public Guid? MaxMultiplierGameId { get; init; }
} 