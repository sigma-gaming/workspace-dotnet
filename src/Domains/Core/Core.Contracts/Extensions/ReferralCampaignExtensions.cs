using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferralCampaignExtensions
{
    public static ReferralCampaignData ToData(this ReferralCampaign entity)
    {
        return new ReferralCampaignData
        {
            CreatedAt = entity.CreatedAt,
            Name = entity.Name,
            Code = entity.Code,
            TotalVisits = entity.TotalVisits,
            TotalSignups = entity.TotalSignups,
            ReferrerId = entity.ReferrerId,
            Id = entity.Id
        };
    }
} 