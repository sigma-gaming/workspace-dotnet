using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class AccountExtensions
{
    public static Account ToData(this AccountEntity entity)
    {
        return new Account
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