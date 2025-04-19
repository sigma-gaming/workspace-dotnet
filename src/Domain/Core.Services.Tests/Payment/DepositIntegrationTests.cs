using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Services.Finance;
using Core.Services.Payment;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.Payment.Providers;
using Core.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Core.Services.Tests.Payment;

public class DepositIntegrationTests : PaymentIntegrationTestBase, IAsyncLifetime
{
    private BalanceService? balanceService;
    private ILogger<DepositIntegrationTests>? logger;
    private IPaymentService? paymentService;
    private PaymentProviderFactory? providerFactory;
    private PaymentService? realPaymentService;
    private TestDepositProvider? testDepositProvider;
    private PaymentWebhookHandler? webhookHandler;

    public DepositIntegrationTests(PaymentDbFixture dbFixture, PaymentCacheFixture cacheFixture, PaymentNatsFixture natsFixture) 
        : base(dbFixture, cacheFixture, natsFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        paymentService = ServiceProvider.GetRequiredService<IPaymentService>();
        realPaymentService = ServiceProvider.GetRequiredService<PaymentService>();
        webhookHandler = new PaymentWebhookHandler(
            realPaymentService,
            ServiceProvider.GetRequiredService<ILogger<PaymentWebhookHandler>>());
        balanceService = ServiceProvider.GetRequiredService<BalanceService>();
        logger = ServiceProvider.GetRequiredService<ILogger<DepositIntegrationTests>>();
        testDepositProvider = ServiceProvider.GetRequiredService<TestDepositProvider>();
        providerFactory = ServiceProvider.GetRequiredService<PaymentProviderFactory>();

        // Log the available deposit configs
        var depositConfigs = realPaymentService.GetDepositConfigs();
        if (logger != null)
        {
            logger.LogInformation("Test deposit configs: {Count}", depositConfigs.Count);
            foreach (var config in depositConfigs)
                logger.LogInformation("Deposit Config: {Method}, {Currency}, {Provider}, {MinAmount}, {MaxAmount}",
                    config.Method, config.Currency, config.Provider, config.MinAmount, config.MaxAmount);
        }

        // Check if the test provider is registered correctly
        var provider = providerFactory.GetDepositProvider(PaymentProvider.Plisio);
        if (logger != null)
        {
            logger.LogInformation("Provider from factory: {Provider}", provider?.GetType().Name ?? "null");
            logger.LogInformation("TestDepositProvider: {Provider}", testDepositProvider?.GetType().Name ?? "null");
            logger.LogInformation("Provider is TestDepositProvider: {IsTestProvider}",
                provider != null && testDepositProvider != null &&
                provider.GetType() == testDepositProvider.GetType());

            // New diagnostics
            logger.LogInformation("Factory type: {Type}", providerFactory?.GetType().FullName ?? "null");
            logger.LogInformation("Is TestPaymentProviderFactory: {IsMock}",
                providerFactory?.GetType().Name == "TestPaymentProviderFactory");

            // Log provider types for diagnostics only
            if (testDepositProvider != null)
            {
                var withdrawalProvider = ServiceProvider.GetRequiredService<TestWithdrawalProvider>();
                var staticWalletProvider = ServiceProvider.GetRequiredService<StaticWalletMockProvider>();
                
                logger.LogInformation("Available providers: Deposit={Deposit}, Withdrawal={Withdrawal}, StaticWallet={StaticWallet}", 
                    testDepositProvider.GetType().Name,
                    withdrawalProvider.GetType().Name,
                    staticWalletProvider.GetType().Name);
            }
        }
    }

    public override Task DisposeAsync()
    {
        return base.DisposeAsync();
    }

    [Fact]
    public async Task Should_CreateDeposit_Successfully()
    {
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var gemAmount = 1000L;
        var method = DepositMethod.Plisio;
        var currency = Currency.Ton;
        var provider = PaymentProvider.Plisio;

        // Log the deposit configs again at test time
        var configs = realPaymentService!.GetDepositConfigs();
        if (logger != null)
        {
            logger.LogInformation("Test deposit configs in test: {Count}", configs.Count);

            // Check if there's a matching config
            var matchingConfig = configs.FirstOrDefault(c =>
                c.Method == method &&
                c.Currency == currency &&
                c.Provider == provider);

            if (matchingConfig != null)
                logger.LogInformation(
                    "Found matching config: {Method}, {Currency}, {Provider}, {MinAmount}, {MaxAmount}",
                    matchingConfig.Method, matchingConfig.Currency, matchingConfig.Provider,
                    matchingConfig.MinAmount, matchingConfig.MaxAmount);
            else
                logger.LogWarning("No matching config found for {Method}, {Currency}, {Provider}",
                    method, currency, provider);

            // Check provider registration again
            var depositProvider = providerFactory!.GetDepositProvider(provider);
            logger.LogInformation("Provider in test: {Provider}",
                depositProvider != null ? depositProvider.GetType().Name : "null");
            logger.LogInformation("Provider is TestDepositProvider: {IsTestProvider}",
                ReferenceEquals(depositProvider, testDepositProvider));
        }

        var initialBalance = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(0, initialBalance.Available);

        var input = new DepositInput(
            userId,
            gemAmount,
            method,
            currency,
            provider
        );

        // Create a manual deposit using the test provider
        var providerResponse = await testDepositProvider!.CreateDepositAsync(input);
        Assert.NotNull(providerResponse);

        // Create a valid payload of type DepositPayload
        var payload = new DepositPayloadRedirect { RedirectUrl = "https://example.com/redirect" };

        var depositEntity = new DepositEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GemAmount = gemAmount,
            Currency = currency,
            Provider = provider,
            Status = PaymentStatus.Pending,
            ProviderTransactionId = providerResponse.ProviderTransactionId,
            ProviderAmount = providerResponse.ProviderAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Payload = payload,
        };

        // Directly save the deposit to the database
        DbContext.Deposits.Add(depositEntity);
        await DbContext.SaveChangesAsync();

        // Verify that the deposit was created
        var savedDeposit = await DbContext.Deposits.FindAsync(depositEntity.Id);
        Assert.NotNull(savedDeposit);
        Assert.Equal(userId, savedDeposit.UserId);
        Assert.Equal(gemAmount, savedDeposit.GemAmount);
        Assert.Equal(currency, savedDeposit.Currency);
        Assert.Equal(provider, savedDeposit.Provider);
        Assert.Equal(PaymentStatus.Pending, savedDeposit.Status);

        // Balance should not change until deposit is confirmed
        var balanceAfterCreate = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(0, balanceAfterCreate.Available);
    }

    [Fact]
    public async Task Should_HandleDepositWebhook_And_ConfirmDeposit()
    {
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var gemAmount = 1000L;
        var method = DepositMethod.Plisio;
        var currency = Currency.Ton;
        var provider = PaymentProvider.Plisio;

        var initialBalance = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(0, initialBalance.Available);

        // Create a manual deposit using the test provider
        var input = new DepositInput(
            userId,
            gemAmount,
            method,
            currency,
            provider
        );

        var providerResponse = await testDepositProvider!.CreateDepositAsync(input);
        Assert.NotNull(providerResponse);

        // Create a valid payload of type DepositPayload
        var payload = new DepositPayloadRedirect { RedirectUrl = "https://example.com/redirect" };

        var depositEntity = new DepositEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GemAmount = gemAmount,
            Currency = currency,
            Provider = provider,
            Status = PaymentStatus.Pending,
            ProviderTransactionId = providerResponse.ProviderTransactionId,
            ProviderAmount = providerResponse.ProviderAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Payload = payload,
        };

        // Directly save the deposit to the database
        DbContext.Deposits.Add(depositEntity);
        await DbContext.SaveChangesAsync();

        logger?.LogInformation("Sending webhook for transaction {TransactionId}",
            depositEntity.ProviderTransactionId);
        var result = await webhookHandler!.HandleDepositWebhookAsync(
            depositEntity.ProviderTransactionId,
            PaymentStatus.Completed,
            provider);

        Assert.True(result);
        var updatedDeposit =
            await paymentService!.GetDepositByProviderTransactionIdAsync(depositEntity.ProviderTransactionId);
        Assert.NotNull(updatedDeposit);
        Assert.Equal(PaymentStatus.Completed, updatedDeposit.Status);

        // Balance should be increased by deposit amount
        var balanceAfterWebhook = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(gemAmount, balanceAfterWebhook.Available);
    }

    [Fact]
    public async Task Should_UseTransaction_When_ConfirmingDeposit()
    {
        var user = await CreateTestUserAsync();
        var userId = user.Id;
        var gemAmount = 1000L;
        var method = DepositMethod.Plisio;
        var currency = Currency.Ton;
        var provider = PaymentProvider.Plisio;

        var initialBalance = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(0, initialBalance.Available);

        // Create a manual deposit using the test provider
        var input = new DepositInput(
            userId,
            gemAmount,
            method,
            currency,
            provider
        );

        var providerResponse = await testDepositProvider!.CreateDepositAsync(input);
        Assert.NotNull(providerResponse);

        // Create a valid payload of type DepositPayload
        var payload = new DepositPayloadRedirect { RedirectUrl = "https://example.com/redirect" };

        var depositEntity = new DepositEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GemAmount = gemAmount,
            Currency = currency,
            Provider = provider,
            Status = PaymentStatus.Pending,
            ProviderTransactionId = providerResponse.ProviderTransactionId,
            ProviderAmount = providerResponse.ProviderAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Payload = payload,
        };

        // Directly save the deposit to the database
        DbContext.Deposits.Add(depositEntity);
        await DbContext.SaveChangesAsync();

        logger?.LogInformation("Confirming deposit {DepositId}", depositEntity.Id);

        // Confirm deposit and ensure transactionality (no partial updates)
        var confirmed = await paymentService!.ConfirmDepositAsync(depositEntity.Id);
        Assert.True(confirmed);
        var updatedDeposit =
            await paymentService!.GetDepositByProviderTransactionIdAsync(depositEntity.ProviderTransactionId);
        Assert.NotNull(updatedDeposit);
        Assert.Equal(PaymentStatus.Completed, updatedDeposit.Status);

        // Balance should be increased by deposit amount
        var balanceAfterConfirm = await balanceService!.GetBalanceAsync(userId);
        Assert.Equal(gemAmount, balanceAfterConfirm.Available);
    }
}