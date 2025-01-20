using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class ReferrerTransactionExtensions
{
    public static ReferrerTransactionData ToData(this ReferrerTransaction entity)
    {
        return new ReferrerTransactionData
        {
            CreatedAt = entity.CreatedAt,
            Amount = entity.Amount,
            ReferrerId = entity.ReferrerId,
            IsProcessed = entity.IsProcessed,
            ReferralId = entity.ReferralId,
            ReferralCampaignId = entity.ReferralCampaignId,
            Id = entity.Id,
            ReferralAction = entity.ReferralAction
        };
    }
} 