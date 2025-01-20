using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

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