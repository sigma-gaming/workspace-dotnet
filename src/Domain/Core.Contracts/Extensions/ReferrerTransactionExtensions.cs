using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ReferrerTransactionExtensions
{
    public static ReferrerTransaction ToData(this ReferrerTransactionEntity entity)
    {
        return new ReferrerTransaction
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