using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class SessionExtensions
{
    public static Session ToData(this SessionEntity entity)
    {
        return new Session
        {
            ExpiresAt = entity.ExpiresAt,
            UserId = entity.UserId,
            Id = entity.Id,
            ReferrerId = entity.ReferrerId,
            ReferralCampaignId = entity.ReferralCampaignId,
            Provider = entity.Provider
        };
    }
} 