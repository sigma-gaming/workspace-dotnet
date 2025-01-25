using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class BalanceExtensions
{
    public static Balance ToData(this BalanceEntity entity)
    {
        return new Balance
        {
            UserId = entity.UserId,
            Available = entity.Available,
            WageringRequired = entity.WageringRequired,
            TotalBet = entity.TotalBet,
            TotalWon = entity.TotalWon,
            TotalLost = entity.TotalLost,
            TotalBetCount = entity.TotalBetCount,
            MaxWin = entity.MaxWin,
            MaxMultiplier = entity.MaxMultiplier,
            MaxWinGameId = entity.MaxWinGameId,
            MaxMultiplierGameId = entity.MaxMultiplierGameId
        };
    }
} 