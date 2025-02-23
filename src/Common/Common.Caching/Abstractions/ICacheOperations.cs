namespace Common.Caching.Abstractions;

/// <summary>
///     Defines basic cache operations that are common across all cache entity types.
///     This interface provides a foundation for both standard and safe (Try-) operations.
/// </summary>
public interface ICacheOperations
{
    Task<bool> ExistsAsync();
    Task<bool> TryExistsAsync();
    Task DeleteAsync();
    Task TryDeleteAsync();
}

/// <summary>
///     Defines basic cache operations for keyed cache entities.
/// </summary>
/// <typeparam name="TKey">The type of the key used to identify cache entries</typeparam>
public interface IKeyedCacheOperations<in TKey>
{
    Task<bool> ExistsAsync(TKey key);
    Task<bool> TryExistsAsync(TKey key);
    Task DeleteAsync(TKey key);
    Task TryDeleteAsync(TKey key);
}