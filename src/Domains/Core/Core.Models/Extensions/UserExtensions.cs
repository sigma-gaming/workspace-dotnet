using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class UserExtensions
{
    public static UserData ToData(this User entity)
    {
        return new UserData
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