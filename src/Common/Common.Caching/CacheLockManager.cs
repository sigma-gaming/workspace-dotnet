using Common.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Common.Caching;

/// <summary>
///     Implements a distributed lock manager for cache entities using RedLock.net.
/// </summary>
public class CacheLockManager(
    IConnectionMultiplexer redis,
    string resourceKey,
    ILogger? logger = null) : ICacheLockManager
{
    private readonly RedLockFactory redLockFactory = RedLockFactory.Create([new RedLockMultiplexer(redis)]);

    /// <inheritdoc />
    public async Task<ICacheLock> AcquireLockAsync(
        TimeSpan expiryTime,
        TimeSpan waitTime,
        TimeSpan retryTime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var redLock = await redLockFactory.CreateLockAsync(
                resourceKey,
                expiryTime,
                waitTime,
                retryTime,
                cancellationToken);

            if (!redLock.IsAcquired)
                throw new TimeoutException(
                    $"Failed to acquire lock for resource '{resourceKey}' within the specified wait time.");

            return new CacheLock(redLock, resourceKey, logger);
        }
        catch (Exception ex) when (ex is not TimeoutException)
        {
            logger?.LogError(ex, "Error acquiring lock for resource {ResourceKey}", resourceKey);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ICacheLock> TryAcquireLockAsync(
        TimeSpan expiryTime,
        TimeSpan waitTime,
        TimeSpan retryTime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var redLock = await redLockFactory.CreateLockAsync(
                resourceKey,
                expiryTime,
                waitTime,
                retryTime,
                cancellationToken);

            return new CacheLock(redLock, resourceKey, logger);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error trying to acquire lock for resource {ResourceKey}", resourceKey);
            throw;
        }
    }
}