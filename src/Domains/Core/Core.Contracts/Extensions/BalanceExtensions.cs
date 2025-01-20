using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class BalanceExtensions
{
    public static BalanceData ToData(this Balance entity)
    {
        return new BalanceData
        {
            UserId = entity.UserId,
            Available = entity.Available,
            WageringRequired = entity.WageringRequired,
            TotalBet = entity.TotalBet,
            TotalWin = entity.TotalWin,
            TotalLost = entity.TotalLost,
            TotalBetCount = entity.TotalBetCount,
            MaxWin = entity.MaxWin,
            MaxMultiplier = entity.MaxMultiplier,
            MaxWinGameId = entity.MaxWinGameId,
            MaxMultiplierGameId = entity.MaxMultiplierGameId
        };
    }
} 