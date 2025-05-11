namespace Common.Caching.Abstractions;

public interface ILockableKeyedCacheEntity<in TKey>
{
    Task<ICacheLock> AcquireLockAsync(
        TKey key,
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default);
}