using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class SessionExtensions
{
    public static SessionData ToData(this Session entity)
    {
        return new SessionData
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