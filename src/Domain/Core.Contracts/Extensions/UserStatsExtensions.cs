using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class UserStatsExtensions
{
    public static UserStats ToData(this UserStatsEntity entity)
    {
        return new UserStats
        {
            UserId = entity.UserId,
            DepositCount = entity.DepositCount,
            WithdrawCount = entity.WithdrawCount
        };
    }
} 