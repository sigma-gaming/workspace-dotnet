using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class ReferrerPayoutExtensions
{
    public static ReferrerPayoutData ToData(this ReferrerPayout entity)
    {
        return new ReferrerPayoutData
        {
            ReferrerId = entity.ReferrerId,
            NextPayoutAt = entity.NextPayoutAt,
            LastPayoutAt = entity.LastPayoutAt
        };
    }
} 