using Common.Caching;
using Core.Cache.Configuration;
using Core.Contracts.Data;
using StackExchange.Redis;

namespace Core.Services.Caching;

public class CoreCacheRegistry(IConnectionMultiplexer redis, RedisCacheOptions options)
{
    public KeyedJsonCacheEntity<Guid, Balance> UserBalance { get; } = new(
        redis,
        userId => $"{options.Version}:balance:{userId}",
        TimeSpan.FromHours(1));
}