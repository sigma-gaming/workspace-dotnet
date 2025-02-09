using Core.Contracts.Entities;

namespace Core.Services.Gaming.Runners;

public sealed record GamePlayResult
{
    public required GameRecordEntity GameRecord { get; init; }
    public required BalanceEntity UpdatedBalance { get; init; }
}