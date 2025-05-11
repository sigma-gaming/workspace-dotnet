using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Caching;

public abstract class BaseCacheOperations(IConnectionMultiplexer redis, ILogger logger)
{
    private bool IsReady()
    {
        return redis.IsConnected;
    }

    protected async Task<TResult?> ExecuteWithSafetyAsync<TResult>(
        Func<Task<TResult>> operation,
        TResult? defaultValue)
    {
        if (!IsReady()) return defaultValue;

        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while executing cache operation");
            return defaultValue;
        }
    }

    protected async Task ExecuteWithSafetyAsync(Func<Task> operation)
    {
        if (!IsReady()) return;

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while executing cache operation");
        }
    }

    protected CacheLockManager CreateLockManager(string resourceKey)
    {
        return new CacheLockManager(redis, $"lock:{resourceKey}", logger);
    }
}