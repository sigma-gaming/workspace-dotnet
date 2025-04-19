using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;

namespace Core.Tests.Fixtures;

/// <summary>
/// Redis fixture for integration tests
/// </summary>
public class CoreCacheFixture : IAsyncLifetime, IDisposable
{
    private readonly RedisContainer redisContainer;
    private readonly ILogger<CoreCacheFixture> logger;
    private readonly string databasePrefix;
    private bool isDisposed;

    /// <summary>
    /// Redis connection for tests
    /// </summary>
    public IConnectionMultiplexer Connection { get; private set; } = null!;

    /// <summary>
    /// Connection string for Redis
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new Redis fixture with an optional database prefix for isolation
    /// </summary>
    /// <param name="databasePrefix">Prefix for Redis keys to isolate test data (optional)</param>
    public CoreCacheFixture(string? databasePrefix = null)
    {
        this.databasePrefix = databasePrefix ?? $"test:{Guid.NewGuid():N}";

        // Configure Redis container
        redisContainer = new RedisBuilder()
            .WithImage("redis:7.2-alpine")
            .WithCleanUp(true)
            .Build();

        // Create a logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<CoreCacheFixture>();
    }

    /// <summary>
    /// Initialize the Redis container and create connection
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Start container
            logger.LogInformation("Starting Redis container...");
            await redisContainer.StartAsync();
            ConnectionString = redisContainer.GetConnectionString();

            // Connect to Redis
            Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
            logger.LogInformation("Redis fixture initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Redis fixture");
            throw;
        }
    }

    /// <summary>
    /// Get Redis database with the test prefix for key isolation
    /// </summary>
    public IDatabase GetDatabase(int db = 0) => Connection.GetDatabase(db);

    /// <summary>
    /// Generate a Redis key with the test prefix for isolation
    /// </summary>
    public string GetPrefixedKey(string key) => $"{databasePrefix}:{key}";

    /// <summary>
    /// Clear all keys with the test prefix
    /// </summary>
    public async Task ClearTestKeysAsync()
    {
        try
        {
            logger.LogInformation("Clearing Redis test keys with prefix: {Prefix}", databasePrefix);
            
            var server = Connection.GetServer(Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{databasePrefix}:*").ToArray();
            
            if (keys.Length > 0)
            {
                var db = GetDatabase();
                await db.KeyDeleteAsync(keys);
                logger.LogInformation("Cleared {Count} Redis keys", keys.Length);
            }
            else
            {
                logger.LogInformation("No keys to clear");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clear Redis test keys");
            throw;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;
        
        if (disposing)
        {
            Connection?.Dispose();
        }
        
        isDisposed = true;
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~CoreCacheFixture()
    {
        Dispose(false);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            // Try to clear test keys before shutting down
            await ClearTestKeysAsync();
            
            // Close Redis connection
            if (Connection != null)
            {
                await Connection.CloseAsync();
            }
            
            // Stop container
            await redisContainer.StopAsync();
            await redisContainer.DisposeAsync();
            
            logger.LogInformation("Redis fixture disposed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disposing Redis fixture");
        }
    }
} 