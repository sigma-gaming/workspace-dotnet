using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

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