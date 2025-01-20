using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

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