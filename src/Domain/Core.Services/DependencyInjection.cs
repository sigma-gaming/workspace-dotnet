using Common.Caching;
using Common.Caching.Abstractions;
using Common.PubSub;
using Common.PubSub.Abstractions;
using Core.Builder;
using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Chat;
using Core.Services.Finance;
using Core.Services.Finance.Api;
using Core.Services.Gaming;
using Core.Services.Gaming.Runners;
using Core.Services.Notifications;
using Core.Services.Promotions;
using Core.Services.Promotions.TaskCheckers;
using Core.Services.PubSub;
using Core.Services.System;
using Core.Services.Telegram;
using Core.Services.User;
using Core.Services.Vk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class DependencyInjection
{
    public static CoreBuilder AddCommonServices(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddSingleton<CoreCacheRegistry>();

        builder.Services.AddHttpClient<BinanceApi>();
        builder.Services.AddHttpClient<CoincapApi>();

        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProfileService>();
        builder.Services.AddScoped<UserStatsService>();
        builder.Services.AddScoped<FraudService>();

        builder.Services.Configure<SessionOptions>(configuration.GetSection("Session"));
        builder.Services.AddOptionsWithValidateOnStart<SessionOptions>();
        builder.Services.AddScoped<SessionManager>();
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<AuthService>();

        builder.Services.AddScoped<BalanceService>();
        builder.Services.AddScoped<BudgetService>();
        builder.Services.AddScoped<CurrencyRatesService>();

        builder.Services.AddScoped<GameService>();
        builder.Services.AddScoped<GameHistoryService>();
        builder.Services.AddScoped<DiceGame>();
        builder.Services.AddScoped<PincodeGame>();

        builder.Services.AddScoped<AffiliateService>();
        builder.Services.AddScoped<PromocodeService>();

        return builder;
    }

    public static CoreBuilder AddSystem(this CoreBuilder builder)
    {
        builder.Services.AddSingleton<DomainService>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<DomainService>());
        builder.Services.AddScoped<MaintenanceService>();

        return builder;
    }

    public static CoreBuilder AddPubSub(this CoreBuilder builder)
    {
        builder.Services.AddSingleton<IPubSubService, NatsPubSubService>();
        builder.Services.AddSingleton<CorePubSubRegistry>();

        return builder;
    }

    public static CoreBuilder AddChat(this CoreBuilder builder)
    {
        builder.Services.AddScoped<ChatService>();

        return builder;
    }

    public static CoreBuilder AddNotifications(this CoreBuilder builder)
    {
        builder.Services.AddScoped<NotificationService>();

        return builder;
    }

    public static CoreBuilder AddTelegram(this CoreBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));
        builder.Services.AddOptionsWithValidateOnStart<TelegramOptions>();
        builder.Services.AddSingleton<TelegramService>();

        return builder;
    }

    public static CoreBuilder AddVk(this CoreBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<VkOptions>(configuration.GetSection("Vk"));
        builder.Services.AddOptionsWithValidateOnStart<VkOptions>();

        return builder;
    }

    public static CoreBuilder AddGlobalTasks(this CoreBuilder builder)
    {
        builder.Services.AddScoped<GlobalTaskService>();
        builder.Services.AddScoped<TelegramGroupSubscribeChecker>();
        builder.Services.AddScoped<VkGroupSubscribeChecker>();
        builder.Services.AddScoped<VkPinnedRepostChecker>();
        builder.Services.AddScoped<GlobalTaskCheckerProvider>();

        return builder;
    }
}