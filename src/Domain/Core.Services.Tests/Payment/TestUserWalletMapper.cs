using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Mappers;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Implementation of UserWalletMapper for tests
/// </summary>
public class TestUserWalletMapper : UserWalletMapper
{
    /// <summary>
    /// Maps a user wallet entity to a user wallet data object
    /// </summary>
    public new UserWallet ToData(UserWalletEntity entity)
    {
        return new UserWallet
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Provider = entity.Provider,
            Currency = entity.Currency,
            WalletAddress = entity.WalletAddress,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsActive = entity.IsActive
        };
    }
} 