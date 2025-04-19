using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Tests.Fixtures;

/// <summary>
/// Extension methods for working with multiple fixtures together
/// </summary>
public static class FixtureCollectionExtensions
{
    /// <summary>
    /// Create a service collection with the PostgreSQL fixture
    /// </summary>
    public static IServiceCollection AddPostgreSqlFixture(this IServiceCollection services, CoreDbFixture fixture)
    {
        // Register connection string from the fixture
        services.AddSingleton(fixture);
        
        // Add DbContext factory through the fixture
        services.AddSingleton(provider => fixture.DbContextFactory);
        
        return services;
    }
    
    /// <summary>
    /// Create a service collection with the Redis fixture
    /// </summary>
    public static IServiceCollection AddRedisFixture(this IServiceCollection services, CoreCacheFixture fixture)
    {
        // Register Redis connection from the fixture
        services.AddSingleton(fixture);
        services.AddSingleton(fixture.Connection);
        
        return services;
    }
    
    /// <summary>
    /// Create a service collection with the NATS fixture
    /// </summary>
    public static IServiceCollection AddNatsFixture(this IServiceCollection services, CoreNatsFixture fixture)
    {
        // Register NATS connection from the fixture
        services.AddSingleton(fixture);
        services.AddSingleton(fixture.Connection);
        
        return services;
    }
    
    /// <summary>
    /// Add all fixtures to a service collection
    /// </summary>
    public static IServiceCollection AddAllFixtures(
        this IServiceCollection services, 
        CoreDbFixture postgresFixture,
        CoreCacheFixture redisFixture,
        CoreNatsFixture natsFixture)
    {
        services.AddPostgreSqlFixture(postgresFixture);
        services.AddRedisFixture(redisFixture);
        services.AddNatsFixture(natsFixture);
        
        return services;
    }
    
    /// <summary>
    /// Add logging to a service collection
    /// </summary>
    public static IServiceCollection AddTestLogging(this IServiceCollection services, LogLevel minLevel = LogLevel.Information)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(minLevel);
        });
        
        return services;
    }
} 