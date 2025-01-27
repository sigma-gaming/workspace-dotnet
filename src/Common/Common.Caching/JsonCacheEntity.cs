using System.Text.Json;
using Common.Caching.Abstractions;
using StackExchange.Redis;

namespace Common.Caching;

public class JsonCacheEntity<T>(
    IConnectionMultiplexer redis,
    string key,
    TimeSpan? ttl = null)
    : IJsonCacheEntity<T>
{
    public async Task<T?> GetAsync()
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue
            ? JsonSerializer.Deserialize<T>(value!)
            : default;
    }

    public async Task SetAsync(T value)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, ttl);
    }

    public async Task DeleteAsync()
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}

public class KeyedJsonCacheEntity<TKey, TValue>(
    IConnectionMultiplexer redis,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null)
    : IKeyedJsonCacheEntity<TKey, TValue>
{
    public async Task<TValue?> GetAsync(TKey key)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue
            ? JsonSerializer.Deserialize<TValue>(value!)
            : default;
    }

    public async Task SetAsync(TKey key, TValue value)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(keyGenerator(key), json, ttl);
    }

    public async Task DeleteAsync(TKey key)
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(keyGenerator(key));
    }
}

public class JsonListCacheEntity<T>(
    IConnectionMultiplexer redis,
    string key,
    int maxItems = 100,
    TimeSpan? ttl = null)
    : IListCacheEntity<T>
{
    public async Task<List<T>> GetAllAsync()
    {
        var db = redis.GetDatabase();
        var values = await db.ListRangeAsync(key);
        return values
            .Select(x => JsonSerializer.Deserialize<T>(x!))
            .Where(x => x != null)
            .ToList()!;
    }

    public async Task AddAsync(T item)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(item);

        await db.ListLeftPushAsync(key, json);
        await db.ListTrimAsync(key, 0, maxItems - 1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task<int> GetCountAsync()
    {
        var db = redis.GetDatabase();
        return (int)await db.ListLengthAsync(key);
    }

    public async Task ClearAsync()
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    public async Task DeleteAsync()
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}

public class KeyedJsonListCacheEntity<TKey, TValue>(
    IConnectionMultiplexer redis,
    Func<TKey, string> keyGenerator,
    int maxItems = 100,
    TimeSpan? ttl = null)
    : IKeyedListCacheEntity<TKey, TValue>
{
    public async Task<List<TValue>> GetAllAsync(TKey key)
    {
        var db = redis.GetDatabase();
        var values = await db.ListRangeAsync(keyGenerator(key));
        return values
            .Select(x => JsonSerializer.Deserialize<TValue>(x!))
            .Where(x => x != null)
            .ToList()!;
    }

    public async Task AddAsync(TKey key, TValue item)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(item);

        await db.ListLeftPushAsync(keyGenerator(key), json);
        await db.ListTrimAsync(keyGenerator(key), 0, maxItems - 1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(keyGenerator(key), ttl);
    }

    public async Task<int> GetCountAsync(TKey key)
    {
        var db = redis.GetDatabase();
        return (int)await db.ListLengthAsync(keyGenerator(key));
    }

    public async Task ClearAsync(TKey key)
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task DeleteAsync(TKey key)
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(keyGenerator(key));
    }
}