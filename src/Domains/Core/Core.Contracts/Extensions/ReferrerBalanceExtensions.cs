using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerBalanceExtensions
{
    public static ReferrerBalance ToData(this ReferrerBalanceEntity entity)
    {
        return new ReferrerBalance
        {
            ReferrerId = entity.ReferrerId,
            Available = entity.Available
        };
    }
} 