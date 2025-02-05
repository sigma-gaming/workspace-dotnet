using Core.Builder;
using Core.Services.Auth;
using Core.Services.Bots;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.Finance.Api;
using Core.Services.Gaming;
using Core.Services.Notifications;
using Core.Services.Promotions;
using Core.Services.PubSub;
using Core.Services.System;
using Core.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class DependencyInjection
{
    public static CoreBuilder AddCommonServices(
        this CoreBuilder builder)
    {
        builder.Services.AddSingleton<CoreCacheRegistry>();
        builder.Services.AddSingleton<CorePubSubRegistry>();

        builder.Services.AddHttpClient<BinanceApi>();
        builder.Services.AddHttpClient<CoincapApi>();

        builder.Services.AddScoped<MaintenanceService>();
        builder.Services.AddSingleton<DomainService>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<DomainService>());

        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ProfileService>();
        builder.Services.AddScoped<UserStatsService>();
        builder.Services.AddScoped<FraudService>();

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<SessionService>();

        builder.Services.AddScoped<BalanceService>();
        builder.Services.AddScoped<BudgetService>();
        builder.Services.AddScoped<CurrencyRatesService>();

        builder.Services.AddScoped<GameService>();
        builder.Services.AddScoped<GameHistoryService>();

        builder.Services.AddScoped<AffiliateService>();
        builder.Services.AddScoped<GlobalTaskService>();
        builder.Services.AddScoped<PromocodeService>();

        builder.Services.AddScoped<NotificationService>();

        return builder;
    }

    public static CoreBuilder AddTelegram(this CoreBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));
        builder.Services.AddSingleton<TelegramService>();

        return builder;
    }
}