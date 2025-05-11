namespace Common.Caching.Abstractions;

public interface IIntegerCacheEntity : ICacheOperations, ILockableCacheEntity
{
    Task<long?> GetAsync();
    Task<long?> TryGetAsync();
    Task SetAsync(long value);
    Task TrySetAsync(long value);
    Task IncrementByAsync(long value);
    Task TryIncrementByAsync(long value);
    Task DecrementByAsync(long value);
    Task TryDecrementByAsync(long value);
}

public interface IKeyedIntegerCacheEntity<in TKey> : IKeyedCacheOperations<TKey>, ILockableKeyedCacheEntity<TKey>
{
    Task<long?> GetAsync(TKey key);
    Task<long?> TryGetAsync(TKey key);
    Task SetAsync(TKey key, long value);
    Task TrySetAsync(TKey key, long value);
    Task IncrementByAsync(TKey key, long value);
    Task TryIncrementByAsync(TKey key, long value);
    Task DecrementByAsync(TKey key, long value);
    Task TryDecrementByAsync(TKey key, long value);
}

public interface IStringCacheEntity : ICacheOperations, ILockableCacheEntity
{
    Task<string?> GetAsync();
    Task<string?> TryGetAsync();
    Task SetAsync(string value);
    Task TrySetAsync(string value);
}

public interface IKeyedStringCacheEntity<in TKey> : IKeyedCacheOperations<TKey>, ILockableKeyedCacheEntity<TKey>
{
    Task<string?> GetAsync(TKey key);
    Task<string?> TryGetAsync(TKey key);
    Task SetAsync(TKey key, string value);
    Task TrySetAsync(TKey key, string value);
}

public interface IBooleanCacheEntity : ICacheOperations, ILockableCacheEntity
{
    Task<bool?> GetAsync();
    Task<bool?> TryGetAsync();
    Task SetAsync(bool value);
    Task TrySetAsync(bool value);
    Task ToggleAsync();
    Task TryToggleAsync();
}

public interface IKeyedBooleanCacheEntity<in TKey> : IKeyedCacheOperations<TKey>, ILockableKeyedCacheEntity<TKey>
{
    Task<bool?> GetAsync(TKey key);
    Task<bool?> TryGetAsync(TKey key);
    Task SetAsync(TKey key, bool value);
    Task TrySetAsync(TKey key, bool value);
    Task ToggleAsync(TKey key);
    Task TryToggleAsync(TKey key);
}