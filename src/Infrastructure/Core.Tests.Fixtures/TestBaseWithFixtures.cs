using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Core.Tests.Fixtures;

/// <summary>
/// Base class for integration tests using fixtures
/// </summary>
public abstract class TestBaseWithFixtures : IAsyncLifetime
{
    /// <summary>
    /// Service provider for test dependencies
    /// </summary>
    protected ServiceProvider ServiceProvider { get; private set; } = null!;
    
    /// <summary>
    /// Service scope for test dependencies
    /// </summary>
    protected IServiceScope ServiceScope { get; private set; } = null!;
    
    /// <summary>
    /// Logger for test output
    /// </summary>
    protected ILogger Logger { get; private set; } = null!;

    /// <summary>
    /// Initialize the test
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        // Initialize services
        var services = new ServiceCollection();
        await ConfigureServicesAsync(services);
        ServiceProvider = services.BuildServiceProvider();
        ServiceScope = ServiceProvider.CreateScope();
        
        // Get logger
        var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger(GetType());
        
        // Initialize test data
        await InitializeTestDataAsync();
    }

    /// <summary>
    /// Configure services for the test
    /// </summary>
    protected abstract Task ConfigureServicesAsync(IServiceCollection services);
    
    /// <summary>
    /// Initialize test data - override in derived classes
    /// </summary>
    protected virtual Task InitializeTestDataAsync() => Task.CompletedTask;

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        ServiceScope.Dispose();
        await ServiceProvider.DisposeAsync();
    }
    
    /// <summary>
    /// Get a required service from the service provider
    /// </summary>
    protected T GetRequiredService<T>() where T : notnull => 
        ServiceScope.ServiceProvider.GetRequiredService<T>();
}

/// <summary>
/// Base class for tests using PostgreSQL, Redis, and NATS fixtures
/// </summary>
public abstract class IntegrationTestBase : TestBaseWithFixtures, IClassFixture<CoreDbFixture>, IClassFixture<CoreCacheFixture>, IClassFixture<CoreNatsFixture>
{
    private readonly CoreDbFixture postgresFixture;
    private readonly CoreCacheFixture redisFixture;
    private readonly CoreNatsFixture natsFixture;

    /// <summary>
    /// Create a new integration test with all fixtures
    /// </summary>
    protected IntegrationTestBase(CoreDbFixture postgresFixture, CoreCacheFixture redisFixture, CoreNatsFixture natsFixture)
    {
        this.postgresFixture = postgresFixture;
        this.redisFixture = redisFixture;
        this.natsFixture = natsFixture;
    }

    /// <summary>
    /// Configure services with all fixtures
    /// </summary>
    protected override Task ConfigureServicesAsync(IServiceCollection services)
    {
        // Register fixtures
        services.AddAllFixtures(postgresFixture, redisFixture, natsFixture);
        
        // Add logging
        services.AddTestLogging();
        
        // Allow derived classes to add their services
        ConfigureAdditionalServices(services);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Configure additional services - override in derived classes
    /// </summary>
    protected virtual void ConfigureAdditionalServices(IServiceCollection services)
    {
    }
    
    /// <summary>
    /// Create a database helper for transaction management
    /// </summary>
    protected DatabaseHelper<TContext> CreateDatabaseHelper<TContext>() where TContext : DbContext
    {
        var factory = ServiceProvider.GetRequiredService<IDbContextFactory<TContext>>();
        return new DatabaseHelper<TContext>(factory, Logger);
    }
    
    /// <summary>
    /// Get and reset the Redis database by clearing test keys
    /// </summary>
    protected async Task ResetRedisAsync()
    {
        await redisFixture.ClearTestKeysAsync();
    }
    
    /// <summary>
    /// Reset the database state for a clean test
    /// </summary>
    protected async Task ResetDatabaseAsync()
    {
        await postgresFixture.ResetDatabaseAsync();
    }
} 