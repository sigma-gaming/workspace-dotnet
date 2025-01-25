using Common.Caching;
using Core.Cache.Configuration;
using Core.Contracts.Data;
using StackExchange.Redis;

namespace Core.Services.Caching;

public class CoreCacheRegistry(IConnectionMultiplexer redis, RedisCacheOptions options)
{
    public KeyedJsonCacheEntity<Guid, Session> Session { get; } = new(
        redis,
        sessionId => $"{options.Version}:session:{sessionId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, Balance> UserBalance { get; } = new(
        redis,
        userId => $"{options.Version}:balance:{userId}",
        TimeSpan.FromHours(1));
}