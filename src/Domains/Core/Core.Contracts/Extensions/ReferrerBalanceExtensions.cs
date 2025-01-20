using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerBalanceExtensions
{
    public static ReferrerBalanceData ToData(this ReferrerBalance entity)
    {
        return new ReferrerBalanceData
        {
            ReferrerId = entity.ReferrerId,
            Available = entity.Available
        };
    }
} 