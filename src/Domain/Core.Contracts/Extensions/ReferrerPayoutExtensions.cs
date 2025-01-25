using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerPayoutExtensions
{
    public static ReferrerPayout ToData(this ReferrerPayoutEntity entity)
    {
        return new ReferrerPayout
        {
            ReferrerId = entity.ReferrerId,
            NextPayoutAt = entity.NextPayoutAt,
            LastPayoutAt = entity.LastPayoutAt
        };
    }
} 