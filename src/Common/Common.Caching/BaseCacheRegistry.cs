using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Caching;

public abstract class BaseCacheRegistry(IConnectionMultiplexer redis, ILogger logger)
{
    public bool IsReady()
    {
        return redis.IsConnected;
    }

    public async Task ExecuteIfConnectedAsync(Func<Task> operation)
    {
        if (!IsReady())
        {
            logger.LogWarning("Cache is not ready, skipping operation.");
            return;
        }

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while executing cache operation.");
        }
    }
}