using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Db.Context;
using Core.Services.Payment;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Core.Services.Auth;
using Core.Contracts.Mappers;
using Core.Services.User;
using Core.Services.Promotions;
using System.Net.Http;
using StackExchange.Redis;
using Common.PubSub.Abstractions;
using Common.Db.Extensions;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Base class for Payment service integration tests using Core test fixtures
/// </summary>
[Collection("PaymentTestCollection")]
public abstract class PaymentIntegrationTestBase : IAsyncLifetime
{
    protected readonly PaymentDbFixture DbFixture;
    protected readonly PaymentCacheFixture CacheFixture;
    protected readonly PaymentNatsFixture NatsFixture;
    protected readonly ILogger<PaymentIntegrationTestBase> Logger;
    
    protected ServiceProvider ServiceProvider { get; private set; } = null!;
    
    // Database context for tests
    protected CoreDbContext DbContext { get; private set; } = null!;

    protected PaymentIntegrationTestBase(
        PaymentDbFixture dbFixture, 
        PaymentCacheFixture cacheFixture,
        PaymentNatsFixture natsFixture)
    {
        DbFixture = dbFixture;
        CacheFixture = cacheFixture;
        NatsFixture = natsFixture;
        
        // Create logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = loggerFactory.CreateLogger<PaymentIntegrationTestBase>();
    }

    /// <summary>
    /// Setup before each test
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        // Создаем DbContext до конфигурации сервисов, чтобы использовать его как singleton
        DbContext = await CreateDbContextAsync();
        
        // Configure services
        var services = new ServiceCollection();
        await ConfigureServicesAsync(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Configure services for tests
    /// </summary>
    protected virtual async Task ConfigureServicesAsync(IServiceCollection services)
    {
        
        // Register fixtures
        services.AddSingleton(DbFixture);
        
        services.AddSingleton(CacheFixture);
        services.AddSingleton(CacheFixture.Connection);
        
        services.AddSingleton(NatsFixture);
        
        // Регистрируем единый экземпляр DbContext как singleton
        services.AddSingleton(DbContext);
        // Также регистрируем как базовый DbContext для сервисов, которые могут использовать этот интерфейс
        services.AddSingleton<DbContext>(DbContext);
        
        // Configure Redis cache service with the test container
        
        // Add Cache services and options
        services.AddSingleton(provider => {
            var endpoints = CacheFixture.Connection.GetEndPoints();
            var endpoint = endpoints.FirstOrDefault();
            
            if (endpoint == null)
                throw new InvalidOperationException("Redis connection has no endpoints");
                
            var endpointString = endpoint.ToString() ?? "localhost:6379";
            var parts = endpointString.Split(':');
            var host = parts[0];
            var port = parts.Length > 1 ? int.Parse(parts[1]) : 6379;
            
            return new Core.Cache.Configuration.RedisCacheOptions 
            { 
                Host = host,
                Port = port,
                Version = "test"
            };
        });
        
        // Add real cache service connected to test Redis container
        services.AddSingleton<Common.Caching.Abstractions.ICacheService, Common.Caching.CacheService>();
        
        services.AddSingleton<Core.Services.Caching.CoreCacheRegistry>();
        
        // Add mock PubSub service connected to test NATS container
        services.AddScoped<IPubSubService>(provider => 
            new MockPubSubService(
                NatsFixture.Connection,
                provider.GetRequiredService<ILogger<MockPubSubService>>()
            ));
        
        // Add PubSub services
        services.AddSingleton<Core.Services.PubSub.CorePubSubRegistry>();
        
        // Add mappers
        services.AddSingleton<BalanceMapper>();
        
        // COMMENTED: services.AddSingleton<UserWalletMapper, TestUserWalletMapper>();
        services.AddSingleton<UserWalletMapper>();
        
        services.AddSingleton<AccountMapper>();
        
        services.AddSingleton<UserMapper>();
        
        services.AddSingleton<ProfileMapper>();
        
        services.AddSingleton<ReferralCampaignMapper>();
        
        services.AddSingleton<ReferrerSettingsMapper>();
        
        services.AddSingleton<ReferrerBalanceMapper>();
        
        services.AddSingleton<ReferrerPayoutMapper>();
        
        services.AddSingleton<ReferrerWithdrawalMapper>();
        
        // Add HttpClient for AuthService
        services.AddSingleton<HttpClient>();
        
        // Add Core services
        services.AddScoped<AuthService>();
        
        services.AddScoped<UserService>();
        
        services.AddScoped<AffiliateService>();
        
        services.AddScoped<Core.Services.Finance.BalanceService>();
        
        // Register test payment providers
        services.AddScoped<TestWithdrawalProvider>();
        
        services.AddScoped<TestDepositProvider>();
        
        services.AddScoped<StaticWalletMockProvider>();
        
        services.AddScoped<IPaymentProviderService>(provider => 
            provider.GetRequiredService<TestWithdrawalProvider>());
        
        services.AddScoped<IWithdrawalProvider>(provider => 
            provider.GetRequiredService<TestWithdrawalProvider>());
            
        services.AddScoped<IPaymentProviderService>(provider => 
            provider.GetRequiredService<TestDepositProvider>());
        
        services.AddScoped<IDepositProvider>(provider => 
            provider.GetRequiredService<TestDepositProvider>());
            
        services.AddScoped<IPaymentProviderService>(provider => 
            provider.GetRequiredService<StaticWalletMockProvider>());
        
        services.AddScoped<IStaticWalletProvider>(provider => 
            provider.GetRequiredService<StaticWalletMockProvider>());
        
        // Add test payment provider factory
        services.AddScoped<TestPaymentProviderFactory>();
        
        services.AddScoped<PaymentProviderFactory>(provider => 
            provider.GetRequiredService<TestPaymentProviderFactory>());
        
        // Add configuration providers
        services.AddSingleton<TestDepositConfigProvider>();
        
        services.AddSingleton<IDepositConfigProvider, TestDepositConfigProvider>();
            
        services.AddSingleton<TestWithdrawalConfigProvider>();
        
        services.AddSingleton<IWithdrawalConfigProvider, TestWithdrawalConfigProvider>();
        
        // Register real PaymentService for tests
        services.AddScoped<Core.Services.Payment.PaymentService>();
        
        // Add our test payment service extension
        services.AddScoped<TestPaymentService>();
        
        // Register as interface implementation - use our extension
        services.AddScoped<IPaymentService>(provider => 
            provider.GetRequiredService<TestPaymentService>());
        
        // Register wallet service
        services.AddScoped<Core.Services.Payment.WalletService>();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Initialize database schema
        await InitializeDatabaseAsync();
        
    }

    /// <summary>
    /// Initialize the database schema
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        // Create a fresh DbContext to apply schema
        await using var context = await DbFixture.CreateDbContextAsync();
        
        // Apply migrations instead of manual table creation
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Get a service from DI container
    /// </summary>
    protected T GetService<T>() where T : notnull => 
        ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Reset database between tests
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        // Clear Redis cache
        await CacheFixture.ClearTestKeysAsync();
        
        // Dispose context
        await DbContext.DisposeAsync();
        
        // Dispose service provider
        await ServiceProvider.DisposeAsync();
    }
    
    /// <summary>
    /// Create a new DbContext
    /// </summary>
    protected async Task<CoreDbContext> CreateDbContextAsync() => 
        await DbFixture.CreateDbContextAsync();
    
    /// <summary>
    /// Execute code within transaction with automatic rollback
    /// </summary>
    protected async Task ExecuteWithRollbackAsync(Func<CoreDbContext, Task> action)
    {
        // Используем ExecuteInTransactionAsync вместо собственной логики
        try
        {
            await DbContext.ExecuteInTransactionAsync(async () =>
            {
                await action(DbContext);
                
                // Генерируем исключение, чтобы откатить транзакцию
                throw new TransactionAbortedException("Rollback requested");
            });
        }
        catch (TransactionAbortedException)
        {
            // Игнорируем специальное исключение для отката
        }
    }
        
    /// <summary>
    /// Execute code within transaction with automatic rollback and return value
    /// </summary>
    protected async Task<T> ExecuteWithRollbackAsync<T>(Func<CoreDbContext, Task<T>> func)
    {
        T result = default!;
        
        try
        {
            await DbContext.ExecuteInTransactionAsync(async () =>
            {
                result = await func(DbContext);
                
                // Генерируем исключение, чтобы откатить транзакцию
                throw new TransactionAbortedException("Rollback requested");
            });
        }
        catch (TransactionAbortedException)
        {
            // Игнорируем специальное исключение для отката
        }
        
        return result;
    }
    
    /// <summary>
    /// Execute code with a transaction but don't roll it back (for test setup)
    /// </summary>
    protected async Task ExecuteWithoutRollbackAsync(Func<CoreDbContext, Task> action)
    {
        await DbContext.ExecuteInTransactionAsync(async () =>
        {
            await action(DbContext);
        });
    }
    
    /// <summary>
    /// Execute code with a transaction but don't roll it back (for test setup) with return value
    /// </summary>
    protected async Task<T> ExecuteWithoutRollbackAsync<T>(Func<CoreDbContext, Task<T>> func)
    {
        return await DbContext.ExecuteInTransactionAsync(async () =>
        {
            return await func(DbContext);
        });
    }
    
    /// <summary>
    /// Create a test user for integration tests
    /// </summary>
    protected async Task<UserEntity> CreateTestUserAsync(bool virtualUser = false)
    {
        var authService = GetService<AuthService>();
        
        // Create authentication payload with test data
        var payload = new AuthenticatePayload
        {
            UserId = null, // New user
            Provider = AccountProvider.Telegram, // Using Telegram as test provider
            ProviderUserId = Guid.NewGuid().ToString(), // Unique ID
            ProviderUsername = $"test_{Guid.NewGuid().ToString()[..8]}",
            ProviderUserFirstName = "Test",
            ProviderUserLastName = "User",
            ProviderUserImage = null,
            ReferralCampaignCode = null,
            Virtual = virtualUser
        };
        
        // Authenticate (create) the user
        var result = await authService.AuthenticateAsync(payload);
        
        if (result.Outcome != AuthOutcome.SignedUp || result.User == null)
        {
            throw new InvalidOperationException($"Failed to create test user. Outcome: {result.Outcome}");
        }
        
        // Fetch and return the actual user entity
        var userEntity = await DbContext.Users.FirstAsync(u => u.Id == result.User.Id);
        
        Logger.LogInformation("Created test user with ID: {UserId}", userEntity.Id);
        
        return userEntity;
    }
    
    /// <summary>
    /// Add funds to a user's balance
    /// </summary>
    protected async Task AddFundsToUserBalanceAsync(Guid userId, long amount)
    {
        await DbContext.ExecuteInTransactionAsync(async () =>
        {
            var balance = await DbContext.Balances.FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (balance == null)
            {
                balance = new BalanceEntity
                {
                    UserId = userId,
                    Available = amount,
                    WageringRequired = 0,
                    TotalBet = 0,
                    TotalWon = 0,
                    TotalLost = 0,
                    TotalBetCount = 0,
                    MaxWin = 0,
                    MaxMultiplier = 0,
                };
                DbContext.Balances.Add(balance);
            }
            else
            {
                balance.Available += amount;
            }
            
            var transaction = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Type = TransactionType.Deposit,
                CreatedAt = DateTime.UtcNow,
            };
            
            DbContext.Transactions.Add(transaction);
            await DbContext.SaveChangesAsync();
            
            Logger.LogInformation("Added {Amount} to user {UserId} balance", amount, userId);
        });
    }
    
    /// <summary>
    /// Get the current balance for a user, refreshing from the database
    /// </summary>
    protected async Task<Balance> GetFreshBalanceAsync(Guid userId)
    {
        var balanceEntity = await DbContext.Balances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId);
        
        if (balanceEntity == null)
        {
            return new Balance
            {
                UserId = userId,
                Available = 0,
                WageringRequired = 0,
                TotalBet = 0,
                TotalWon = 0,
                TotalLost = 0,
                TotalBetCount = 0,
                MaxWin = 0,
                MaxMultiplier = 0,
            };
        }
        
        return new Balance
        {
            UserId = balanceEntity.UserId,
            Available = balanceEntity.Available,
            WageringRequired = balanceEntity.WageringRequired,
            TotalBet = balanceEntity.TotalBet,
            TotalWon = balanceEntity.TotalWon,
            TotalLost = balanceEntity.TotalLost,
            TotalBetCount = balanceEntity.TotalBetCount,
            MaxWin = balanceEntity.MaxWin,
            MaxMultiplier = balanceEntity.MaxMultiplier,
            MaxWinGameId = balanceEntity.MaxWinGameId,
            MaxMultiplierGameId = balanceEntity.MaxMultiplierGameId,
        };
    }
} 

// Специальное исключение для отката транзакций
public class TransactionAbortedException : Exception 
{
    public TransactionAbortedException(string message) : base(message) { }
} 