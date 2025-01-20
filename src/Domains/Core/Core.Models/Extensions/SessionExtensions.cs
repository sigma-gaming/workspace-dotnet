using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

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