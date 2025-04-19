using Core.Contracts.Enums;
using Core.Services.Payment;
using Core.Services.Payment.Exceptions;
using Core.Services.Payment.Models;
using Core.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using InvalidOp = Core.Services.Payment.Exceptions.InvalidOperationException;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Tests for payment service error handling
/// </summary>
public class ExceptionHandlingTests : PaymentIntegrationTestBase, IAsyncLifetime
{
    private PaymentService? paymentService;
    private WalletService? walletService;

    public ExceptionHandlingTests(PaymentDbFixture dbFixture, PaymentCacheFixture cacheFixture, PaymentNatsFixture natsFixture) 
        : base(dbFixture, cacheFixture, natsFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        paymentService = ServiceProvider.GetRequiredService<PaymentService>();
        walletService = ServiceProvider.GetRequiredService<WalletService>();
    }

    public override Task DisposeAsync()
    {
        return base.DisposeAsync();
    }

    [Fact]
    public async Task CreateDeposit_WithUnsupportedMethod_ShouldReturnUnsupportedMethodOutcome()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;

        var input = new DepositInput(
            userId,
            100L,
            DepositMethod.CryptoCloud,
            Currency.UsdtTrc20,
            PaymentProvider.CryptoCloud
        );

        // Act
        var result = await paymentService!.CreateDepositAsync(input);

        // Assert
        Assert.Equal(PaymentOutcome.UnsupportedMethod, result.Outcome);
        Assert.NotNull(result.Error);
        Assert.Contains("Provider CryptoCloud", result.Error);
    }

    [Fact]
    public async Task CreateDeposit_WithInvalidAmount_ShouldReturnInvalidAmountOutcome()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;

        // Get deposit configs to find a valid method
        var configs = paymentService!.GetDepositConfigs();
        Assert.NotEmpty(configs);

        var config = configs[0];

        var input = new DepositInput(
            userId,
            config.MaxAmount + 1000, // Exceed max amount
            config.Method,
            config.Currency,
            config.Provider
        );

        // Act
        var result = await paymentService!.CreateDepositAsync(input);

        // Assert
        Assert.Equal(PaymentOutcome.InvalidAmount, result.Outcome);
        Assert.NotNull(result.Error);
        Assert.Contains("outside the allowed range", result.Error);
        Assert.Equal(config.MinAmount, result.MinAmount);
        Assert.Equal(config.MaxAmount, result.MaxAmount);
    }

    [Fact]
    public async Task CreateWallet_WithUnsupportedProvider_ShouldThrowAppropriateException()
    {
        // Arrange
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var currency = Currency.UsdtTrc20; // Assuming not supported by our mock provider
        var provider = PaymentProvider.Plisio; // Assuming this doesn't implement IStaticWalletProvider

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnsupportedOperationException>(() =>
            walletService!.CreateWalletAsync(userId, currency, provider));

        Assert.Equal(provider, ex.Provider);
        Assert.Contains("Static wallets", ex.Operation);
    }

    [Fact]
    public void TransactionNotFound_ShouldReturnProperException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act - Directly test exception factory
        var ex = ExceptionFactory.TransactionNotFound(nonExistentId, "Deposit");

        // Assert
        Assert.Equal(nonExistentId.ToString(), ex.Identifier);
        Assert.Equal("Deposit", ex.Type);
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void InvalidStatusTransition_ShouldReturnProperException()
    {
        // Act - Directly test exception factory
        var ex = ExceptionFactory.InvalidStatusTransition("deposit", PaymentStatus.Completed, "refund");

        // Assert
        Assert.IsType<InvalidOp>(ex);
        Assert.Equal("StatusTransition", ex.OperationName);
        Assert.Contains("Cannot refund deposit with status Completed", ex.Message);
    }

    [Fact]
    public async Task ConfirmDeposit_WithNonexistentId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentDepositId = Guid.NewGuid();

        // Act
        var result = await paymentService!.ConfirmDepositAsync(nonExistentDepositId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CancelDeposit_WithNonexistentId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentDepositId = Guid.NewGuid();

        // Act
        var result = await paymentService!.CancelDepositAsync(nonExistentDepositId);

        // Assert
        Assert.False(result);
    }
}