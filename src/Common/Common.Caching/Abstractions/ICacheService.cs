namespace Common.Caching.Abstractions;

/// <summary>
///     Provides factory methods for creating various types of cache entities.
///     This service centralizes the creation of cache entities and handles dependency injection.
/// </summary>
public interface ICacheService
{
    // JSON cache entities
    IJsonCacheEntity<T> CreateJson<T>(string key, TimeSpan? ttl = null) where T : class;

    IKeyedJsonCacheEntity<TKey, TValue> CreateKeyedJson<TKey, TValue>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null) where TValue : class;

    IListCacheEntity<T> CreateList<T>(
        string key,
        int maxItems = 100,
        TimeSpan? ttl = null) where T : class;

    IKeyedListCacheEntity<TKey, TValue> CreateKeyedList<TKey, TValue>(
        Func<TKey, string> keyGenerator,
        int maxItems = 100,
        TimeSpan? ttl = null) where TValue : class;

    // Primitive cache entities
    IIntegerCacheEntity CreateInteger(string key, TimeSpan? ttl = null);

    IKeyedIntegerCacheEntity<TKey> CreateKeyedInteger<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null);

    IStringCacheEntity CreateString(string key, TimeSpan? ttl = null);

    IKeyedStringCacheEntity<TKey> CreateKeyedString<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null);

    IBooleanCacheEntity CreateBoolean(string key, TimeSpan? ttl = null);

    IKeyedBooleanCacheEntity<TKey> CreateKeyedBoolean<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null);
        
    /// <summary>
    /// Creates a cache lock manager for the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The resource key to create a lock manager for.</param>
    /// <returns>A cache lock manager instance.</returns>
    ICacheLockManager CreateLockManager(string resourceKey);
}