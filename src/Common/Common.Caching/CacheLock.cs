using Common.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using RedLockNet;

namespace Common.Caching;

/// <summary>
///     Implements a distributed lock for cache entities using RedLock.net.
/// </summary>
internal class CacheLock(IRedLock redLock, string resource, ILogger? logger = null) : ICacheLock
{
    private bool isDisposed;

    /// <inheritdoc />
    public bool IsAcquired => redLock.IsAcquired;

    /// <inheritdoc />
    public string Resource { get; } = resource;

    /// <inheritdoc />
    public async Task ReleaseAsync()
    {
        if (isDisposed)
        {
            logger?.LogWarning("Attempted to release an already disposed lock for resource {Resource}", Resource);
            return;
        }

        try
        {
            await redLock.DisposeAsync();
            isDisposed = true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error releasing lock for resource {Resource}", Resource);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!isDisposed) await ReleaseAsync();

        GC.SuppressFinalize(this);
    }
}