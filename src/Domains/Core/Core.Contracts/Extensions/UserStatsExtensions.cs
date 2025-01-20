using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class UserStatsExtensions
{
    public static UserStatsData ToData(this UserStats entity)
    {
        return new UserStatsData
        {
            UserId = entity.UserId,
            DepositCount = entity.DepositCount,
            WithdrawCount = entity.WithdrawCount
        };
    }
} 