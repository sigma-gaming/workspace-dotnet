using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class UserExtensions
{
    public static User ToData(this UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ProfileId = entity.ProfileId,
            SecurityId = entity.SecurityId,
            ReferrerId = entity.ReferrerId,
            ReferralCampaignId = entity.ReferralCampaignId,
            Virtual = entity.Virtual,
            Roles = entity.Roles
        };
    }
} 