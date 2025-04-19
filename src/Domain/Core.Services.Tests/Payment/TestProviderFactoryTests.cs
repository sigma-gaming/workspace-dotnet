using Core.Contracts.Enums;
using Core.Services.Payment.Interfaces;
using Core.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Services.Tests.Payment;

public class TestProviderFactoryTests : PaymentIntegrationTestBase, IAsyncLifetime
{
    private TestPaymentProviderFactory testProviderFactory = null!;

    public TestProviderFactoryTests(PaymentDbFixture dbFixture, PaymentCacheFixture cacheFixture, PaymentNatsFixture natsFixture) 
        : base(dbFixture, cacheFixture, natsFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        testProviderFactory = ServiceProvider.GetRequiredService<TestPaymentProviderFactory>();
    }

    public override Task DisposeAsync()
    {
        return base.DisposeAsync();
    }

    [Fact]
    public void Should_GetCorrectTestProvider_ByIdentifier()
    {
        // Arrange & Act - Get deposit test provider using specific test identifier
        var depositProvider = testProviderFactory.GetProviderByTestId<IDepositProvider>(
            PaymentProvider.Plisio,
            "DepositTest");

        // Assert
        Assert.NotNull(depositProvider);
        Assert.IsType<TestDepositProvider>(depositProvider);
        Assert.Equal("DepositTest", ((ITestPaymentProvider)depositProvider).TestIdentifier);
    }

    [Fact]
    public void Should_GetCorrectWithdrawalProvider_ByIdentifier()
    {
        // Arrange & Act - Get withdrawal test provider using specific test identifier
        var withdrawalProvider = testProviderFactory.GetProviderByTestId<IWithdrawalProvider>(
            PaymentProvider.Plisio,
            "WithdrawalTest");

        // Assert
        Assert.NotNull(withdrawalProvider);
        Assert.IsType<TestWithdrawalProvider>(withdrawalProvider);
        Assert.Equal("WithdrawalTest", ((ITestPaymentProvider)withdrawalProvider).TestIdentifier);
    }

    [Fact]
    public void Should_GetStaticWalletProvider_ByIdentifier()
    {
        // Arrange & Act - Get static wallet test provider using specific test identifier
        var staticWalletProvider = testProviderFactory.GetProviderByTestId<IStaticWalletProvider>(
            PaymentProvider.CryptoCloud,
            "StaticWalletTest");

        // Assert
        Assert.NotNull(staticWalletProvider);
        Assert.IsType<StaticWalletMockProvider>(staticWalletProvider);
        Assert.Equal("StaticWalletTest", ((ITestPaymentProvider)staticWalletProvider).TestIdentifier);
    }

    [Fact]
    public void Should_ThrowException_WhenTestIdentifierNotFound()
    {
        // Act & Assert - Should throw when test identifier doesn't exist
        Assert.Throws<InvalidOperationException>(() =>
            testProviderFactory.GetProviderByTestId<IDepositProvider>(
                PaymentProvider.Plisio,
                "NonExistingIdentifier"));
    }
}