using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class GlobalTaskExtensions
{
    public static GlobalTaskData ToData(this GlobalTask entity)
    {
        return new GlobalTaskData
        {
            Key = entity.Key,
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive,
            Requirements = entity.Requirements,
            Payout = entity.Payout,
            WageringMultiplier = entity.WageringMultiplier
        };
    }
} 