using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Payment.Exceptions;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Payment;

public class WalletService(
    CoreDbContext db,
    ILogger<WalletService> logger,
    PaymentProviderFactory providerFactory,
    UserWalletMapper walletMapper)
{
    /// <summary>
    ///     Gets all active wallets for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user wallets</returns>
    public async Task<List<UserWallet>> GetUserWalletsAsync(Guid userId)
    {
        try
        {
            var wallets = await db.UserWallets
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return wallets.Select(walletMapper.ToData).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallets for user {UserId}", userId);
            throw new WalletException($"Failed to get wallets for user {userId}", ex);
        }
    }

    /// <summary>
    ///     Gets a user wallet by ID
    /// </summary>
    /// <param name="walletId">Wallet ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>User wallet if found, null otherwise</returns>
    public async Task<UserWallet?> GetUserWalletAsync(Guid walletId, Guid userId)
    {
        try
        {
            var wallet = await db.UserWallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId && w.IsActive);

            return wallet != null ? walletMapper.ToData(wallet) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallet {WalletId} for user {UserId}", walletId, userId);
            throw new WalletException($"Failed to get wallet {walletId} for user {userId}", ex);
        }
    }

    /// <summary>
    ///     Gets a user wallet by currency and provider
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currency">Currency</param>
    /// <param name="provider">Payment provider</param>
    /// <returns>User wallet if found, null otherwise</returns>
    public async Task<UserWallet?> GetUserWalletByCurrencyAsync(Guid userId, Currency currency,
        PaymentProvider provider)
    {
        try
        {
            var wallet = await db.UserWallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    w.Currency == currency &&
                    w.Provider == provider &&
                    w.IsActive);

            return wallet != null ? walletMapper.ToData(wallet) : null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallet for user {UserId}, currency {Currency}, provider {Provider}",
                userId, currency, provider);
            throw new WalletException(userId, currency, provider, "Failed to get wallet", ex);
        }
    }

    /// <summary>
    ///     Creates a new wallet for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currency">Currency</param>
    /// <param name="provider">Payment provider</param>
    /// <returns>Created wallet</returns>
    public async Task<UserWallet> CreateWalletAsync(Guid userId, Currency currency, PaymentProvider provider)
    {
        try
        {
            // Check if the user already has a wallet for this currency and provider
            var existingWallet = await db.UserWallets
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    w.Currency == currency &&
                    w.Provider == provider &&
                    w.IsActive);

            if (existingWallet != null) return walletMapper.ToData(existingWallet);

            // Get the provider service
            var paymentProvider = providerFactory.GetPaymentProvider(provider);
            if (paymentProvider == null) throw new PaymentProviderException(provider, "Provider is not supported");

            // Check if the provider supports static wallets
            if (paymentProvider is not IStaticWalletProvider staticWalletProvider)
                throw new UnsupportedOperationException(provider, "Static wallets");

            // Check if the provider supports static wallets for this currency
            if (!staticWalletProvider.SupportsStaticWallets(currency))
                throw new UnsupportedOperationException(provider, $"Static wallets for currency {currency}");

            // Generate wallet address
            var walletAddress = await staticWalletProvider.GenerateWalletAddressAsync(currency, userId);

            // Create wallet entity
            var wallet = new UserWalletEntity
            {
                UserId = userId,
                Provider = provider,
                Currency = currency,
                WalletAddress = walletAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            db.UserWallets.Add(wallet);
            await db.SaveChangesAsync();

            return walletMapper.ToData(wallet);
        }
        catch (PaymentException)
        {
            // Re-throw payment exceptions for proper handling by caller
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create wallet for user {UserId}, currency {Currency}, provider {Provider}",
                userId, currency, provider);
            throw new WalletException(userId, currency, provider, "Failed to create wallet", ex);
        }
    }

    /// <summary>
    ///     Gets or creates a wallet for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currency">Currency</param>
    /// <param name="provider">Payment provider</param>
    /// <returns>User wallet</returns>
    public async Task<UserWallet?> GetOrCreateWalletAsync(Guid userId, Currency currency, PaymentProvider provider)
    {
        try
        {
            var wallet = await GetUserWalletByCurrencyAsync(userId, currency, provider);

            if (wallet == null) wallet = await CreateWalletAsync(userId, currency, provider);

            return wallet;
        }
        catch (PaymentException ex)
        {
            logger.LogWarning(ex,
                "Payment error while getting or creating wallet for user {UserId}, currency {Currency}, provider {Provider}",
                userId, currency, provider);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while getting or creating wallet for user {UserId}, currency {Currency}, provider {Provider}",
                userId, currency, provider);
            return null;
        }
    }
}