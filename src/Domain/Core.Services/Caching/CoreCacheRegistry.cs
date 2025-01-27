using Common.Caching;
using Common.Caching.Abstractions;
using Core.Cache.Configuration;
using Core.Contracts.Compounds;
using Core.Contracts.Data;
using StackExchange.Redis;

namespace Core.Services.Caching;

public class CoreCacheRegistry(IConnectionMultiplexer redis, RedisCacheOptions options)
{
    public bool Ready()
    {
        return redis.IsConnected;
    }

    public BooleanCacheEntity MaintenanceEnabled { get; } = new(redis, "global:maintenanceEnabled");
    public BooleanCacheEntity BackgroundJobsEnabled { get; } = new(redis, "global:backgroundJobsEnabled");

    public JsonCacheEntity<Budget> Budget { get; } = new(
        redis,
        $"{options.Version}:global:budget",
        TimeSpan.FromMinutes(15));

    public IntegerCacheEntity BudgetAvailable { get; } = new(
        redis,
        "global:budgetAvailable",
        TimeSpan.FromDays(1));

    public KeyedJsonCacheEntity<Guid, Session> Session { get; } = new(
        redis,
        sessionId => $"{options.Version}:session:{sessionId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, User> User { get; } = new(
        redis,
        userId => $"{options.Version}:user:{userId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, ProfileDetailed> DetailedProfile { get; } = new(
        redis,
        userId => $"{options.Version}:detailedProfile:{userId}",
        TimeSpan.FromHours(1));

    public KeyedStringCacheEntity<Guid> SessionCodeToSessionId { get; } = new(
        redis,
        sessionCode => $"sessionCodeToToken:{sessionCode}",
        TimeSpan.FromMinutes(5));

    public KeyedJsonCacheEntity<Guid, Balance> UserBalance { get; } = new(
        redis,
        userId => $"{options.Version}:balance:{userId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, ReferrerBalance> ReferrerBalance { get; } = new(
        redis,
        userId => $"{options.Version}:referrerBalance:{userId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, ReferrerSettings> ReferrerSettings { get; } = new(
        redis,
        userId => $"{options.Version}:referrerSettings:{userId}",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, LastReferrerTransactions> LastReferrerTransactions { get; } = new(
        redis,
        userId => $"{options.Version}:lastReferrerTransactions:{userId}",
        TimeSpan.FromMinutes(15));

    public JsonCacheEntity<Notification[]> GlobalNotifications { get; } = new(
        redis,
        $"{options.Version}:global:notifications",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<Guid, Notification[]> PersonalNotifications { get; } = new(
        redis,
        userId => $"{options.Version}:notifications:{userId}",
        TimeSpan.FromHours(1));

    public JsonListCacheEntity<ChatMessage> LastChatMessages { get; } = new(
        redis,
        $"{options.Version}:global:lastChatMessages",
        50,
        TimeSpan.FromDays(1));

    public JsonListCacheEntity<GameRecord> LastWinHistory { get; } = new(
        redis,
        $"{options.Version}:global:lastWinHistory",
        10,
        TimeSpan.FromHours(6));

    public JsonListCacheEntity<GameRecord> BigWinHistory { get; } = new(
        redis,
        $"{options.Version}:global:bigWinHistory",
        10,
        TimeSpan.FromHours(6));

    public KeyedJsonListCacheEntity<Guid, GameRecord> UserGameHistory { get; } = new(
        redis,
        userId => $"{options.Version}:userGameHistory:{userId}",
        10,
        TimeSpan.FromHours(6));

    public KeyedJsonCacheEntity<string, Promocode> Promocode { get; } = new(
        redis,
        code => $"{options.Version}:promocode:{code}",
        TimeSpan.FromHours(1));

    public JsonCacheEntity<GlobalTask[]> GlobalTasks { get; } = new(
        redis,
        $"{options.Version}:global:globalTasks",
        TimeSpan.FromHours(1));

    public KeyedJsonCacheEntity<string, GlobalTaskStatus> GlobalTaskStatus { get; } = new(
        redis,
        keyAndUserId => $"{options.Version}:globalTaskStatus:{keyAndUserId}",
        TimeSpan.FromHours(1));

    public JsonCacheEntity<CurrencyExchangeRates> CurrencyExchangeRates { get; } = new(
        redis,
        $"global:currencyRates",
        TimeSpan.FromMinutes(15));

    public StringCacheEntity LastMetricsTransactionId { get; } = new(
        redis,
        "global:lastMetricsTransactionId");
}