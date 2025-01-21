namespace Common.Caching.Abstractions;

public interface INumberCacheEntity
{
    Task<double?> GetAsync();
    Task SetAsync(double value);
    Task IncrementByAsync(double value);
    Task DecrementByAsync(double value);
}

public interface IKeyedNumberCacheEntity<TKey>
{
    Task<double?> GetAsync(TKey key);
    Task SetAsync(TKey key, double value);
    Task IncrementByAsync(TKey key, double value);
    Task DecrementByAsync(TKey key, double value);
}

public interface IStringCacheEntity
{
    Task<string?> GetAsync();
    Task SetAsync(string value);
}

public interface IKeyedStringCacheEntity<TKey>
{
    Task<string?> GetAsync(TKey key);
    Task SetAsync(TKey key, string value);
}

public interface IBooleanCacheEntity
{
    Task<bool?> GetAsync();
    Task SetAsync(bool value);
    Task ToggleAsync();
}

public interface IKeyedBooleanCacheEntity<TKey>
{
    Task<bool?> GetAsync(TKey key);
    Task SetAsync(TKey key, bool value);
    Task ToggleAsync(TKey key);
} 