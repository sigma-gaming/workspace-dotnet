using System.Text.Json;
using Common.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Caching;

public class JsonCacheEntity<T>(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IJsonCacheEntity<T> where T : class
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync()
    {
        return await db.KeyExistsAsync(key);
    }

    public async Task<T?> GetAsync()
    {
        var value = await db.StringGetAsync(key);
        return value.HasValue
            ? JsonSerializer.Deserialize<T>(value!)
            : null;
    }

    public async Task SetAsync(T value)
    {
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, ttl);
    }

    public async Task DeleteAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    public async Task<bool> TryExistsAsync()
    {
        return await ExecuteWithSafetyAsync(ExistsAsync, false);
    }

    public async Task<T?> TryGetAsync()
    {
        return await ExecuteWithSafetyAsync(GetAsync, null);
    }

    public async Task TrySetAsync(T value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(value));
    }

    public async Task TryDeleteAsync()
    {
        await ExecuteWithSafetyAsync(DeleteAsync);
    }

    public Task<ICacheLock> AcquireLockAsync(
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default)
    {
        var lockManager = CreateLockManager(key);

        return lockManager.AcquireLockAsync(
            expiryTime ?? TimeSpan.FromSeconds(30),
            waitTime ?? TimeSpan.FromSeconds(10),
            retryTime ?? TimeSpan.FromMilliseconds(200),
            cancellationToken);
    }
}

public class KeyedJsonCacheEntity<TKey, TValue>(
    IConnectionMultiplexer redis,
    ILogger logger,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IKeyedJsonCacheEntity<TKey, TValue>
    where TValue : class
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync(TKey key)
    {
        return await db.KeyExistsAsync(keyGenerator(key));
    }

    public async Task<TValue?> GetAsync(TKey key)
    {
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue
            ? JsonSerializer.Deserialize<TValue>(value!)
            : null;
    }

    public async Task SetAsync(TKey key, TValue value)
    {
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(keyGenerator(key), json, ttl);
    }

    public async Task DeleteAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<bool> TryExistsAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => ExistsAsync(key), false);
    }

    public async Task<TValue?> TryGetAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => GetAsync(key), null);
    }

    public async Task TrySetAsync(TKey key, TValue value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(key, value));
    }

    public async Task TryDeleteAsync(TKey key)
    {
        await ExecuteWithSafetyAsync(() => DeleteAsync(key));
    }

    public Task<ICacheLock> AcquireLockAsync(
        TKey key,
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default)
    {
        var lockManager = CreateLockManager(keyGenerator(key));

        return lockManager.AcquireLockAsync(
            expiryTime ?? TimeSpan.FromSeconds(30),
            waitTime ?? TimeSpan.FromSeconds(10),
            retryTime ?? TimeSpan.FromMilliseconds(200),
            cancellationToken);
    }
}

public class JsonListCacheEntity<T>(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    int maxItems = 100,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IListCacheEntity<T> where T : class
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync()
    {
        return await db.KeyExistsAsync(key);
    }

    public async Task<List<T>> GetAllAsync()
    {
        var values = await db.ListRangeAsync(key);

        return values
            .Select(x => JsonSerializer.Deserialize<T>(x!))
            .Where(x => x is not null)
            .ToList()!;
    }

    public async Task SetAllAsync(List<T> items)
    {
        var serialized = items.Select(x => (RedisValue)JsonSerializer.Serialize(x)).ToArray();
        var transaction = db.CreateTransaction();

#pragma warning disable CS4014
        transaction.KeyDeleteAsync(key);
        transaction.ListRightPushAsync(key, serialized);
        if (ttl.HasValue)
            transaction.KeyExpireAsync(key, ttl);
#pragma warning restore CS4014

        var succeed = await transaction.ExecuteAsync();
        if (!succeed)
            throw new Exception("Failed to set all items");
    }

    public async Task LeftPushAsync(T item)
    {
        var json = JsonSerializer.Serialize(item);
        await db.ListLeftPushAsync(key, json);
        await db.ListTrimAsync(key, 0, maxItems - 1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task RightPushAsync(T item)
    {
        var json = JsonSerializer.Serialize(item);
        await db.ListRightPushAsync(key, json);
        await db.ListTrimAsync(key, -maxItems, -1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task<int> GetCountAsync()
    {
        return (int)await db.ListLengthAsync(key);
    }

    public async Task ClearAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    public async Task DeleteAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    public async Task<bool> TryExistsAsync()
    {
        return await ExecuteWithSafetyAsync(ExistsAsync, false);
    }

    public async Task<(List<T> list, bool succeed)> TryGetAllAsync()
    {
        var list = await ExecuteWithSafetyAsync(GetAllAsync, null);
        return (list ?? [], list is not null);
    }

    public async Task TrySetAllAsync(List<T> items)
    {
        await ExecuteWithSafetyAsync(() => SetAllAsync(items));
    }

    public async Task TryLeftPushAsync(T item)
    {
        await ExecuteWithSafetyAsync(() => LeftPushAsync(item));
    }

    public async Task TryRightPushAsync(T item)
    {
        await ExecuteWithSafetyAsync(() => RightPushAsync(item));
    }

    public async Task<(int count, bool succeed)> TryGetCountAsync()
    {
        var count = await ExecuteWithSafetyAsync(GetCountAsync, -1);
        return (Math.Max(0, count), count != -1);
    }

    public async Task TryClearAsync()
    {
        await ExecuteWithSafetyAsync(ClearAsync);
    }

    public async Task TryDeleteAsync()
    {
        await ExecuteWithSafetyAsync(DeleteAsync);
    }

    public Task<ICacheLock> AcquireLockAsync(
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default)
    {
        var lockManager = CreateLockManager(key);

        return lockManager.AcquireLockAsync(
            expiryTime ?? TimeSpan.FromSeconds(30),
            waitTime ?? TimeSpan.FromSeconds(10),
            retryTime ?? TimeSpan.FromMilliseconds(200),
            cancellationToken);
    }
}

public class KeyedJsonListCacheEntity<TKey, TValue>(
    IConnectionMultiplexer redis,
    ILogger logger,
    Func<TKey, string> keyGenerator,
    int maxItems = 100,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IKeyedListCacheEntity<TKey, TValue>
    where TValue : class
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync(TKey key)
    {
        return await db.KeyExistsAsync(keyGenerator(key));
    }

    public async Task DeleteAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<List<TValue>> GetAllAsync(TKey key)
    {
        var values = await db.ListRangeAsync(keyGenerator(key));

        return values
            .Select(x => JsonSerializer.Deserialize<TValue>(x!))
            .Where(x => x is not null)
            .ToList()!;
    }

    public async Task SetAllAsync(TKey key, List<TValue> items)
    {
        var cacheKey = keyGenerator(key);
        var serialized = items.Select(x => (RedisValue)JsonSerializer.Serialize(x)).ToArray();
        var transaction = db.CreateTransaction();

#pragma warning disable CS4014
        transaction.KeyDeleteAsync(cacheKey);
        transaction.ListRightPushAsync(cacheKey, serialized);
        if (ttl.HasValue)
            transaction.KeyExpireAsync(cacheKey, ttl);
#pragma warning restore CS4014

        var succeed = await transaction.ExecuteAsync();
        if (!succeed)
            throw new Exception("Failed to set all items");
    }

    public async Task LeftPushAsync(TKey key, TValue item)
    {
        var cacheKey = keyGenerator(key);
        var json = JsonSerializer.Serialize(item);
        await db.ListLeftPushAsync(cacheKey, json);
        await db.ListTrimAsync(cacheKey, 0, maxItems - 1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(cacheKey, ttl);
    }

    public async Task RightPushAsync(TKey key, TValue item)
    {
        var cacheKey = keyGenerator(key);
        var json = JsonSerializer.Serialize(item);
        await db.ListRightPushAsync(cacheKey, json);
        await db.ListTrimAsync(cacheKey, -maxItems, -1);

        if (ttl.HasValue)
            await db.KeyExpireAsync(cacheKey, ttl);
    }

    public async Task<int> GetCountAsync(TKey key)
    {
        return (int)await db.ListLengthAsync(keyGenerator(key));
    }

    public async Task ClearAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<bool> TryExistsAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => ExistsAsync(key), false);
    }

    public async Task TryDeleteAsync(TKey key)
    {
        await ExecuteWithSafetyAsync(() => DeleteAsync(key));
    }

    public async Task<(List<TValue> list, bool succeed)> TryGetAllAsync(TKey key)
    {
        var list = await ExecuteWithSafetyAsync(() => GetAllAsync(key), null);
        return (list ?? [], list is not null);
    }

    public async Task TrySetAllAsync(TKey key, List<TValue> items)
    {
        await ExecuteWithSafetyAsync(() => SetAllAsync(key, items));
    }

    public async Task TryLeftPushAsync(TKey key, TValue item)
    {
        await ExecuteWithSafetyAsync(() => LeftPushAsync(key, item));
    }

    public async Task TryRightPushAsync(TKey key, TValue item)
    {
        await ExecuteWithSafetyAsync(() => RightPushAsync(key, item));
    }

    public async Task<(int count, bool succeed)> TryGetCountAsync(TKey key)
    {
        var count = await ExecuteWithSafetyAsync(() => GetCountAsync(key), -1);
        return (Math.Max(0, count), count != -1);
    }

    public async Task TryClearAsync(TKey key)
    {
        await ExecuteWithSafetyAsync(() => ClearAsync(key));
    }

    public Task<ICacheLock> AcquireLockAsync(
        TKey key,
        TimeSpan? expiryTime = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null,
        CancellationToken cancellationToken = default)
    {
        var lockManager = CreateLockManager(keyGenerator(key));

        return lockManager.AcquireLockAsync(
            expiryTime ?? TimeSpan.FromSeconds(30),
            waitTime ?? TimeSpan.FromSeconds(10),
            retryTime ?? TimeSpan.FromMilliseconds(200),
            cancellationToken);
    }
}