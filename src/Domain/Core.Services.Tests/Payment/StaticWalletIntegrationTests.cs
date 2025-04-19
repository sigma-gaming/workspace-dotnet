using Core.Contracts.Enums;
using Core.Services.Payment;
using Core.Services.Payment.Exceptions;
using Core.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Integration tests for static wallet functionality
/// </summary>
public class StaticWalletIntegrationTests : PaymentIntegrationTestBase, IAsyncLifetime
{
    private WalletService? walletService;

    public StaticWalletIntegrationTests(PaymentDbFixture dbFixture, PaymentCacheFixture cacheFixture, PaymentNatsFixture natsFixture) 
        : base(dbFixture, cacheFixture, natsFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        walletService = ServiceProvider.GetRequiredService<WalletService>();
    }

    public override Task DisposeAsync()
    {
        return base.DisposeAsync();
    }

    [Fact]
    public async Task Should_GetEmptyWalletList_ForNewUser()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;

        // Act
        var wallets = await walletService!.GetUserWalletsAsync(userId);

        // Assert
        Assert.NotNull(wallets);
        Assert.Empty(wallets);
    }

    [Fact]
    public async Task Should_CreateWallet_Successfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Act
        var wallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Assert
        Assert.NotNull(wallet);
        Assert.Equal(userId, wallet.UserId);
        Assert.Equal(currency, wallet.Currency);
        Assert.Equal(provider, wallet.Provider);
        Assert.NotEmpty(wallet.WalletAddress);
        Assert.True(wallet.IsActive);
    }

    [Fact]
    public async Task Should_GetUserWalletById_Successfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Create a wallet first
        var createdWallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Act
        var retrievedWallet = await walletService!.GetUserWalletAsync(createdWallet.Id, userId);

        // Assert
        Assert.NotNull(retrievedWallet);
        Assert.Equal(createdWallet.Id, retrievedWallet.Id);
        Assert.Equal(userId, retrievedWallet.UserId);
        Assert.Equal(currency, retrievedWallet.Currency);
        Assert.Equal(provider, retrievedWallet.Provider);
        Assert.Equal(createdWallet.WalletAddress, retrievedWallet.WalletAddress);
        Assert.True(retrievedWallet.IsActive);
    }

    [Fact]
    public async Task Should_GetUserWalletByCurrency_Successfully()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Create a wallet first
        var createdWallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Act
        var retrievedWallet = await walletService!.GetUserWalletByCurrencyAsync(userId, currency, provider);

        // Assert
        Assert.NotNull(retrievedWallet);
        Assert.Equal(createdWallet.Id, retrievedWallet.Id);
        Assert.Equal(userId, retrievedWallet.UserId);
        Assert.Equal(currency, retrievedWallet.Currency);
        Assert.Equal(provider, retrievedWallet.Provider);
        Assert.Equal(createdWallet.WalletAddress, retrievedWallet.WalletAddress);
        Assert.True(retrievedWallet.IsActive);
    }

    [Fact]
    public async Task Should_ReturnExistingWallet_WhenCreatingDuplicate()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Create first wallet
        var firstWallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Act - Try to create a duplicate wallet with same currency and provider
        var secondWallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Assert - Should return the existing wallet instead of creating a new one
        Assert.NotNull(secondWallet);
        Assert.Equal(firstWallet.Id, secondWallet.Id);
        Assert.Equal(firstWallet.WalletAddress, secondWallet.WalletAddress);
    }

    [Fact]
    public async Task Should_GetOrCreateWallet_ReturnExisting_WhenExists()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Create wallet first
        var createdWallet = await walletService!.CreateWalletAsync(userId, currency, provider);

        // Act
        var retrievedWallet = await walletService!.GetOrCreateWalletAsync(userId, currency, provider);

        // Assert
        Assert.NotNull(retrievedWallet);
        Assert.Equal(createdWallet.Id, retrievedWallet.Id);
        Assert.Equal(createdWallet.WalletAddress, retrievedWallet.WalletAddress);
    }

    [Fact]
    public async Task Should_GetOrCreateWallet_CreateNew_WhenNotExists()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.CryptoCloud;

        // Act
        var wallet = await walletService!.GetOrCreateWalletAsync(userId, currency, provider);

        // Assert
        Assert.NotNull(wallet);
        Assert.Equal(userId, wallet.UserId);
        Assert.Equal(currency, wallet.Currency);
        Assert.Equal(provider, wallet.Provider);
        Assert.NotEmpty(wallet.WalletAddress);
        Assert.True(wallet.IsActive);
    }

    [Fact]
    public async Task Should_CreateMultipleWallets_ForDifferentCurrencies()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var provider = PaymentProvider.CryptoCloud;

        // Act - Create wallets for different currencies
        var tonWallet = await walletService!.CreateWalletAsync(userId, Currency.Ton, provider);
        var btcWallet = await walletService!.CreateWalletAsync(userId, Currency.Btc, provider);

        // Get all user wallets
        var wallets = await walletService!.GetUserWalletsAsync(userId);

        // Assert
        Assert.NotNull(wallets);
        Assert.Equal(2, wallets.Count);
        Assert.Contains(wallets, w => w.Currency == Currency.Ton);
        Assert.Contains(wallets, w => w.Currency == Currency.Btc);
        Assert.NotEqual(tonWallet.WalletAddress, btcWallet.WalletAddress);
    }

    [Fact]
    public async Task Should_ThrowException_WhenProviderNotSupported()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.Ton;
        var provider = PaymentProvider.Plisio; // Plisio doesn't implement IStaticWalletProvider

        // Act & Assert
        await Assert.ThrowsAsync<UnsupportedOperationException>(() =>
            walletService!.CreateWalletAsync(userId, currency, provider));
    }

    [Fact]
    public async Task Should_ThrowException_WhenCurrencyNotSupported()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.UsdtErc20; // Let's assume this isn't supported by CryptoCloud
        var provider = PaymentProvider.CryptoCloud;

        // Act & Assert
        await Assert.ThrowsAsync<UnsupportedOperationException>(() =>
            walletService!.CreateWalletAsync(userId, currency, provider));
    }
}