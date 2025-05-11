using Common.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Caching;

public class IntegerCacheEntity(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IIntegerCacheEntity
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync()
    {
        return await db.KeyExistsAsync(key);
    }

    public async Task<long?> GetAsync()
    {
        var value = await db.StringGetAsync(key);
        return value.HasValue ? long.Parse(value!) : null;
    }

    public async Task SetAsync(long value)
    {
        await db.StringSetAsync(key, value.ToString(), ttl);
    }

    public async Task IncrementByAsync(long value)
    {
        await db.StringIncrementAsync(key, value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task DecrementByAsync(long value)
    {
        await db.StringDecrementAsync(key, value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(key, ttl);
    }

    public async Task DeleteAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    // Try-methods with error handling and connection checks
    public async Task<bool> TryExistsAsync()
    {
        return await ExecuteWithSafetyAsync(ExistsAsync, false);
    }

    public async Task<long?> TryGetAsync()
    {
        return await ExecuteWithSafetyAsync(GetAsync, null);
    }

    public async Task TrySetAsync(long value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(value));
    }

    public async Task TryIncrementByAsync(long value)
    {
        await ExecuteWithSafetyAsync(() => IncrementByAsync(value));
    }

    public async Task TryDecrementByAsync(long value)
    {
        await ExecuteWithSafetyAsync(() => DecrementByAsync(value));
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

public class KeyedIntegerCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    ILogger logger,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IKeyedIntegerCacheEntity<TKey>
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync(TKey key)
    {
        return await db.KeyExistsAsync(keyGenerator(key));
    }

    public async Task<long?> GetAsync(TKey key)
    {
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? long.Parse(value!) : null;
    }

    public async Task SetAsync(TKey key, long value)
    {
        await db.StringSetAsync(keyGenerator(key), value.ToString(), ttl);
    }

    public async Task IncrementByAsync(TKey key, long value)
    {
        await db.StringIncrementAsync(keyGenerator(key), value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(keyGenerator(key), ttl);
    }

    public async Task DecrementByAsync(TKey key, long value)
    {
        await db.StringDecrementAsync(keyGenerator(key), value);
        if (ttl.HasValue)
            await db.KeyExpireAsync(keyGenerator(key), ttl);
    }

    public async Task DeleteAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<bool> TryExistsAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => ExistsAsync(key), false);
    }

    public async Task<long?> TryGetAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => GetAsync(key), null);
    }

    public async Task TrySetAsync(TKey key, long value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(key, value));
    }

    public async Task TryIncrementByAsync(TKey key, long value)
    {
        await ExecuteWithSafetyAsync(() => IncrementByAsync(key, value));
    }

    public async Task TryDecrementByAsync(TKey key, long value)
    {
        await ExecuteWithSafetyAsync(() => DecrementByAsync(key, value));
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

public class StringCacheEntity(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IStringCacheEntity
{
    private readonly IDatabase db = redis.GetDatabase();

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

    public async Task<bool> ExistsAsync()
    {
        return await db.KeyExistsAsync(key);
    }

    public async Task<string?> GetAsync()
    {
        var value = await db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(string value)
    {
        await db.StringSetAsync(key, value, ttl);
    }

    public async Task DeleteAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    public async Task<bool> TryExistsAsync()
    {
        return await ExecuteWithSafetyAsync(ExistsAsync, false);
    }

    public async Task<string?> TryGetAsync()
    {
        return await ExecuteWithSafetyAsync(GetAsync, null);
    }

    public async Task TrySetAsync(string value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(value));
    }

    public async Task TryDeleteAsync()
    {
        await ExecuteWithSafetyAsync(DeleteAsync);
    }
}

public class KeyedStringCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    ILogger logger,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IKeyedStringCacheEntity<TKey>
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync(TKey key)
    {
        return await db.KeyExistsAsync(keyGenerator(key));
    }

    public async Task<string?> GetAsync(TKey key)
    {
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? value.ToString() : null;
    }

    public async Task SetAsync(TKey key, string value)
    {
        await db.StringSetAsync(keyGenerator(key), value, ttl);
    }

    public async Task DeleteAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<bool> TryExistsAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => ExistsAsync(key), false);
    }

    public async Task<string?> TryGetAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => GetAsync(key), null);
    }

    public async Task TrySetAsync(TKey key, string value)
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

public class BooleanCacheEntity(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IBooleanCacheEntity
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync()
    {
        return await db.KeyExistsAsync(key);
    }

    public async Task<bool?> GetAsync()
    {
        var value = await db.StringGetAsync(key);
        return value.HasValue ? bool.Parse(value!) : null;
    }

    public async Task SetAsync(bool value)
    {
        await db.StringSetAsync(key, value.ToString(), ttl);
    }

    public async Task ToggleAsync()
    {
        var currentValue = await GetAsync();
        if (currentValue.HasValue)
            await SetAsync(!currentValue.Value);
    }

    public async Task DeleteAsync()
    {
        await db.KeyDeleteAsync(key);
    }

    public async Task<bool> TryExistsAsync()
    {
        return await ExecuteWithSafetyAsync(ExistsAsync, false);
    }

    public async Task<bool?> TryGetAsync()
    {
        return await ExecuteWithSafetyAsync(GetAsync, null);
    }

    public async Task TrySetAsync(bool value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(value));
    }

    public async Task TryToggleAsync()
    {
        await ExecuteWithSafetyAsync(ToggleAsync);
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

public class KeyedBooleanCacheEntity<TKey>(
    IConnectionMultiplexer redis,
    ILogger logger,
    Func<TKey, string> keyGenerator,
    TimeSpan? ttl = null
) : BaseCacheOperations(redis, logger), IKeyedBooleanCacheEntity<TKey>
{
    private readonly IDatabase db = redis.GetDatabase();

    public async Task<bool> ExistsAsync(TKey key)
    {
        return await db.KeyExistsAsync(keyGenerator(key));
    }

    public async Task<bool?> GetAsync(TKey key)
    {
        var value = await db.StringGetAsync(keyGenerator(key));
        return value.HasValue ? bool.Parse(value!) : null;
    }

    public async Task SetAsync(TKey key, bool value)
    {
        await db.StringSetAsync(keyGenerator(key), value.ToString(), ttl);
    }

    public async Task ToggleAsync(TKey key)
    {
        var currentValue = await GetAsync(key);
        if (currentValue.HasValue)
            await SetAsync(key, !currentValue.Value);
    }

    public async Task DeleteAsync(TKey key)
    {
        await db.KeyDeleteAsync(keyGenerator(key));
    }

    public async Task<bool> TryExistsAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => ExistsAsync(key), false);
    }

    public async Task<bool?> TryGetAsync(TKey key)
    {
        return await ExecuteWithSafetyAsync(() => GetAsync(key), null);
    }

    public async Task TrySetAsync(TKey key, bool value)
    {
        await ExecuteWithSafetyAsync(() => SetAsync(key, value));
    }

    public async Task TryToggleAsync(TKey key)
    {
        await ExecuteWithSafetyAsync(() => ToggleAsync(key));
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