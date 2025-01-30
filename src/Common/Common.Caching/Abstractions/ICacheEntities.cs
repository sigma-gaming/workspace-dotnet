namespace Common.Caching.Abstractions;

public interface IIntegerCacheEntity
{
    Task<bool> ExistsAsync();
    Task<long?> GetAsync();
    Task SetAsync(long value);
    Task IncrementByAsync(long value);
    Task DecrementByAsync(long value);
    Task DeleteAsync();
}

public interface IKeyedIntegerCacheEntity<TKey>
{
    Task<bool> ExistsAsync(TKey key);
    Task<long?> GetAsync(TKey key);
    Task SetAsync(TKey key, long value);
    Task IncrementByAsync(TKey key, long value);
    Task DecrementByAsync(TKey key, long value);
    Task DeleteAsync(TKey key);
}

public interface IStringCacheEntity
{
    Task<bool> ExistsAsync();
    Task<string?> GetAsync();
    Task SetAsync(string value);
    Task DeleteAsync();
}

public interface IKeyedStringCacheEntity<TKey>
{
    Task<bool> ExistsAsync(TKey key);
    Task<string?> GetAsync(TKey key);
    Task SetAsync(TKey key, string value);
    Task DeleteAsync(TKey key);
}

public interface IBooleanCacheEntity
{
    Task<bool> ExistsAsync();
    Task<bool?> GetAsync();
    Task SetAsync(bool value);
    Task ToggleAsync();
    Task DeleteAsync();
}

public interface IKeyedBooleanCacheEntity<TKey>
{
    Task<bool> ExistsAsync(TKey key);
    Task<bool?> GetAsync(TKey key);
    Task SetAsync(TKey key, bool value);
    Task ToggleAsync(TKey key);
    Task DeleteAsync(TKey key);
} 