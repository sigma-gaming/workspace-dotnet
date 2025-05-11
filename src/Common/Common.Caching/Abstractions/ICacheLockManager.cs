using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Caching.Abstractions;

/// <summary>
/// Provides functionality to acquire, release, and manage distributed locks for cache entities.
/// </summary>
public interface ICacheLockManager
{
    /// <summary>
    /// Acquires a distributed lock for the specified resource.
    /// </summary>
    /// <param name="expiryTime">The expiry time for the lock.</param>
    /// <param name="waitTime">The time to wait for the lock.</param>
    /// <param name="retryTime">The time to wait between retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A cache lock that can be used with 'await using' pattern.</returns>
    Task<ICacheLock> AcquireLockAsync(
        TimeSpan expiryTime,
        TimeSpan waitTime,
        TimeSpan retryTime,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tries to acquire a distributed lock for the specified resource.
    /// </summary>
    /// <param name="expiryTime">The expiry time for the lock.</param>
    /// <param name="waitTime">The time to wait for the lock.</param>
    /// <param name="retryTime">The time to wait between retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A cache lock that can be used with 'await using' pattern. Check IsAcquired to see if the lock was acquired.</returns>
    Task<ICacheLock> TryAcquireLockAsync(
        TimeSpan expiryTime,
        TimeSpan waitTime,
        TimeSpan retryTime,
        CancellationToken cancellationToken = default);
}
