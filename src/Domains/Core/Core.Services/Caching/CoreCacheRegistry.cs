using Common.Caching;
using Core.Cache.Configuration;
using Core.Contracts.Data;
using StackExchange.Redis;

namespace Core.Services.Caching;

public class CoreCacheRegistry(IConnectionMultiplexer redis, RedisCacheOptions options)
{
    public BooleanCacheEntity GlobalMaintenance { get; } = new(
        redis,
        $"{options.Version}:global:maintenance",
        TimeSpan.FromMinutes(5));

    public KeyedBooleanCacheEntity<Guid> UserOnlineStatus { get; } = new(
        redis,
        userId => $"{options.Version}:user:{userId}:online",
        TimeSpan.FromMinutes(1));
    
    public NumberCacheEntity GlobalUserCount { get; } = new(
        redis,
        $"{options.Version}:global:userCount",
        TimeSpan.FromDays(1));

    public KeyedJsonCacheEntity<Guid, Balance> UserBalance { get; } = new(
        redis,
        userId => $"{options.Version}:balance:{userId}",
        TimeSpan.FromHours(1));
    
    public StringCacheEntity GlobalAnnouncement { get; } = new(
        redis,
        $"{options.Version}:global:announcement",
        TimeSpan.FromHours(12));

    public KeyedStringCacheEntity<Guid> UserStatus { get; } = new(
        redis,
        userId => $"{options.Version}:user:{userId}:status",
        TimeSpan.FromHours(1));
    
    public JsonCacheEntity<object> GlobalSettings { get; } = new(
        redis,
        $"{options.Version}:global:settings",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, object> UserSettings { get; } = new(
        redis,
        userId => $"{options.Version}:user:{userId}:settings",
        TimeSpan.FromHours(1));
    
    public JsonListCacheEntity<object> SystemNotifications { get; } = new(
        redis,
        key: $"{options.Version}:global:systemNotifications",
        maxItems: 100,
        ttl: TimeSpan.FromDays(1));
}