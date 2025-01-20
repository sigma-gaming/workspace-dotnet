using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

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