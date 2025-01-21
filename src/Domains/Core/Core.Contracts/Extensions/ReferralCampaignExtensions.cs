using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferralCampaignExtensions
{
    public static ReferralCampaign ToData(this ReferralCampaignEntity entity)
    {
        return new ReferralCampaign
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