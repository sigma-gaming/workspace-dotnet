using Common.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Caching;

public class CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger) : ICacheService
{
    public IJsonCacheEntity<T> CreateJson<T>(string key, TimeSpan? ttl = null) where T : class
    {
        return new JsonCacheEntity<T>(redis, logger, key, ttl);
    }

    public IKeyedJsonCacheEntity<TKey, TValue> CreateKeyedJson<TKey, TValue>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null) where TValue : class
    {
        return new KeyedJsonCacheEntity<TKey, TValue>(redis, logger, keyGenerator, ttl);
    }

    public IListCacheEntity<T> CreateList<T>(
        string key,
        int maxItems = 100,
        TimeSpan? ttl = null) where T : class
    {
        return new JsonListCacheEntity<T>(redis, logger, key, maxItems, ttl);
    }

    public IKeyedListCacheEntity<TKey, TValue> CreateKeyedList<TKey, TValue>(
        Func<TKey, string> keyGenerator,
        int maxItems = 100,
        TimeSpan? ttl = null) where TValue : class
    {
        return new KeyedJsonListCacheEntity<TKey, TValue>(redis, logger, keyGenerator, maxItems, ttl);
    }

    public IIntegerCacheEntity CreateInteger(string key, TimeSpan? ttl = null)
    {
        return new IntegerCacheEntity(redis, logger, key, ttl);
    }

    public IKeyedIntegerCacheEntity<TKey> CreateKeyedInteger<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null)
    {
        return new KeyedIntegerCacheEntity<TKey>(redis, logger, keyGenerator, ttl);
    }

    public IStringCacheEntity CreateString(string key, TimeSpan? ttl = null)
    {
        return new StringCacheEntity(redis, logger, key, ttl);
    }

    public IKeyedStringCacheEntity<TKey> CreateKeyedString<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null)
    {
        return new KeyedStringCacheEntity<TKey>(redis, logger, keyGenerator, ttl);
    }

    public IBooleanCacheEntity CreateBoolean(string key, TimeSpan? ttl = null)
    {
        return new BooleanCacheEntity(redis, logger, key, ttl);
    }

    public IKeyedBooleanCacheEntity<TKey> CreateKeyedBoolean<TKey>(
        Func<TKey, string> keyGenerator,
        TimeSpan? ttl = null)
    {
        return new KeyedBooleanCacheEntity<TKey>(redis, logger, keyGenerator, ttl);
    }

    public ICacheLockManager CreateLockManager(string resourceKey)
    {
        return new CacheLockManager(redis, resourceKey, logger);
    }
}