namespace Common.Caching.Abstractions;

public interface ILockableCacheEntity
{
    Task<ICacheLock> AcquireLockAsync(
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default);
}