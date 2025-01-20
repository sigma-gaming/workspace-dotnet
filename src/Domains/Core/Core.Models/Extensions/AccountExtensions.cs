using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

public static class AccountExtensions
{
    public static AccountData ToData(this Account entity)
    {
        return new AccountData
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            ProviderUserId = entity.ProviderUserId,
            ProviderUsername = entity.ProviderUsername,
            ProviderUserFirstName = entity.ProviderUserFirstName,
            ProviderUserLastName = entity.ProviderUserLastName,
            ProviderUserImage = entity.ProviderUserImage,
            UserId = entity.UserId,
            Provider = entity.Provider
        };
    }
} 