namespace Common.Caching.Abstractions;

/// <summary>
///     Represents a distributed lock that can be acquired on a cache entity.
///     Implements IAsyncDisposable to support 'await using' pattern for automatic release.
/// </summary>
public interface ICacheLock : IAsyncDisposable
{
    /// <summary>
    ///     Gets whether the lock was successfully acquired.
    /// </summary>
    bool IsAcquired { get; }

    /// <summary>
    ///     The resource key that this lock is protecting.
    /// </summary>
    string Resource { get; }

    /// <summary>
    ///     Releases the lock before automatic disposal.
    /// </summary>
    /// <returns>A task that represents the asynchronous release operation.</returns>
    Task ReleaseAsync();
}