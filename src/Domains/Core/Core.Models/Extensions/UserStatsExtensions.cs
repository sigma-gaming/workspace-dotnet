using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

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