using Common.Caching.Abstractions;
using Core.Cache.Configuration;
using Core.Contracts.Compounds;
using Core.Contracts.Data;
using StackExchange.Redis;

namespace Core.Services.Caching;

public class CoreCacheRegistry(IConnectionMultiplexer connection, ICacheService cacheService, RedisCacheOptions options)
{
    public IBooleanCacheEntity MaintenanceEnabled { get; } =
        cacheService.CreateBoolean("global:maintenanceEnabled");

    public IBooleanCacheEntity BackgroundJobsEnabled { get; } =
        cacheService.CreateBoolean("global:backgroundJobsEnabled");

    public IJsonCacheEntity<Budget> Budget { get; } =
        cacheService.CreateJson<Budget>(
            $"{options.Version}:global:budget",
            TimeSpan.FromMinutes(15));

    public IIntegerCacheEntity BudgetAvailable { get; } =
        cacheService.CreateInteger(
            "global:budgetAvailable",
            TimeSpan.FromDays(1));

    public IJsonCacheEntity<Notification[]> GlobalNotifications { get; } =
        cacheService.CreateJson<Notification[]>(
            $"{options.Version}:global:notifications",
            TimeSpan.FromHours(1));

    public IJsonCacheEntity<CurrencyExchangeRates> CurrencyExchangeRates { get; } =
        cacheService.CreateJson<CurrencyExchangeRates>(
            "global:currencyRates",
            TimeSpan.FromMinutes(15));

    public IKeyedJsonCacheEntity<Guid, Session> Session { get; } =
        cacheService.CreateKeyedJson<Guid, Session>(
            sessionId => $"{options.Version}:session:{sessionId}",
            TimeSpan.FromHours(1));

    public IKeyedStringCacheEntity<Guid> SessionCodeToSessionId { get; } =
        cacheService.CreateKeyedString<Guid>(
            sessionCode => $"sessionCodeToToken:{sessionCode}",
            TimeSpan.FromMinutes(5));

    public IKeyedJsonCacheEntity<Guid, Contracts.Data.User> User { get; } =
        cacheService.CreateKeyedJson<Guid, Contracts.Data.User>(
            userId => $"{options.Version}:user:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, UserDetails> UserDetails { get; } =
        cacheService.CreateKeyedJson<Guid, UserDetails>(
            userId => $"{options.Version}:userDetails:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, Notification[]> PersonalNotifications { get; } =
        cacheService.CreateKeyedJson<Guid, Notification[]>(
            userId => $"{options.Version}:notifications:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, Balance> UserBalance { get; } =
        cacheService.CreateKeyedJson<Guid, Balance>(
            userId => $"{options.Version}:balance:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, ReferrerBalance> ReferrerBalance { get; } =
        cacheService.CreateKeyedJson<Guid, ReferrerBalance>(
            userId => $"{options.Version}:referrerBalance:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, ReferrerSettings> ReferrerSettings { get; } =
        cacheService.CreateKeyedJson<Guid, ReferrerSettings>(
            userId => $"{options.Version}:referrerSettings:{userId}",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<Guid, LastReferrerTransactions> LastReferrerTransactions { get; } =
        cacheService.CreateKeyedJson<Guid, LastReferrerTransactions>(
            userId => $"{options.Version}:lastReferrerTransactions:{userId}",
            TimeSpan.FromMinutes(15));

    public IListCacheEntity<ChatMessage> LastChatMessages { get; } =
        cacheService.CreateList<ChatMessage>(
            $"{options.Version}:global:lastChatMessages",
            50,
            TimeSpan.FromDays(1));

    public IListCacheEntity<GameRecord> LastWinHistory { get; } =
        cacheService.CreateList<GameRecord>(
            $"{options.Version}:global:lastWinHistory",
            10,
            TimeSpan.FromHours(6));

    public IListCacheEntity<GameRecord> BigWinHistory { get; } =
        cacheService.CreateList<GameRecord>(
            $"{options.Version}:global:bigWinHistory",
            10,
            TimeSpan.FromHours(6));

    public IKeyedListCacheEntity<Guid, GameRecord> UserGameHistory { get; } =
        cacheService.CreateKeyedList<Guid, GameRecord>(
            userId => $"{options.Version}:userGameHistory:{userId}",
            10,
            TimeSpan.FromHours(6));

    public IKeyedJsonCacheEntity<string, Promocode> Promocode { get; } =
        cacheService.CreateKeyedJson<string, Promocode>(
            code => $"{options.Version}:promocode:{code}",
            TimeSpan.FromHours(1));

    public IJsonCacheEntity<GlobalTask[]> GlobalTasks { get; } =
        cacheService.CreateJson<GlobalTask[]>(
            $"{options.Version}:global:globalTasks",
            TimeSpan.FromHours(1));

    public IKeyedJsonCacheEntity<string, GlobalTaskStatusInfo> GlobalTaskStatus { get; } =
        cacheService.CreateKeyedJson<string, GlobalTaskStatusInfo>(
            keyAndUserId => $"{options.Version}:globalTaskStatus:{keyAndUserId}",
            TimeSpan.FromHours(1));

    public IStringCacheEntity LastMetricsTransactionId { get; } =
        cacheService.CreateString("global:lastMetricsTransactionId");

    public bool IsReady()
    {
        return connection.IsConnected;
    }
}