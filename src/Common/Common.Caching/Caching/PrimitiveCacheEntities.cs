using Common.Caching.Abstractions;
using StackExchange.Redis;

namespace Common.Caching;

public class NumberCacheEntity(
    IConnectionMultiplexer redis,
    string key,
    TimeSpan? ttl = null) : INumberCacheEntity
{
    public async Task<double?> GetAsync()
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? (double?)double.Parse(value!) : null;
    }

    public async Task SetAsync(double value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(key, value.ToString(), ttl);
    }

    public async Task IncrementByAsync(double value)
    {
        var db = redis.GetDatabase();
        await db.StringIncrementAsync(key, value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task DecrementByAsync(double value)
    {
        var db = redis.GetDatabase();
        await db.StringDecrementAsync(key, value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }
}

public class KeyedNumberCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null) : IKeyedNumberCacheEntity<TKey>
{
    public async Task<double?> GetAsync(TKey key)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? (double?)double.Parse(value!) : null;
    }

    public async Task SetAsync(TKey key, double value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(keyGenerator(key), value.ToString(), ttl);
    }

    public async Task IncrementByAsync(TKey key, double value)
    {
        var db = redis.GetDatabase();
        await db.StringIncrementAsync(keyGenerator(key), value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(keyGenerator(key), ttl);
    }

    public async Task DecrementByAsync(TKey key, double value)
    {
        var db = redis.GetDatabase();
        await db.StringDecrementAsync(keyGenerator(key), value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(keyGenerator(key), ttl);
    }
}

public class StringCacheEntity(
    IConnectionMultiplexer redis,
    string key,
    TimeSpan? ttl = null) : IStringCacheEntity
{
    public async Task<string?> GetAsync()
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(key, value, ttl);
    }
}

public class KeyedStringCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null) : IKeyedStringCacheEntity<TKey>
{
    public async Task<string?> GetAsync(TKey key)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(TKey key, string value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(keyGenerator(key), value, ttl);
    }
}

public class BooleanCacheEntity(
    IConnectionMultiplexer redis,
    string key,
    TimeSpan? ttl = null) : IBooleanCacheEntity
{
    public async Task<bool?> GetAsync()
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? bool.Parse(value!) : null;
    }

    public async Task SetAsync(bool value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(key, value.ToString(), ttl);
    }

    public async Task ToggleAsync()
    {
        var currentValue = await GetAsync();
        if (currentValue.HasValue)
            await SetAsync(!currentValue.Value);
    }
}

public class KeyedBooleanCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null) : IKeyedBooleanCacheEntity<TKey>
{
    public async Task<bool?> GetAsync(TKey key)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? bool.Parse(value!) : null;
    }

    public async Task SetAsync(TKey key, bool value)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(keyGenerator(key), value.ToString(), ttl);
    }

    public async Task ToggleAsync(TKey key)
    {
        var currentValue = await GetAsync(key);
        if (currentValue.HasValue)
            await SetAsync(key, !currentValue.Value);
    }
} 