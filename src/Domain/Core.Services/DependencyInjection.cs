using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.Finance.Api;
using Core.Services.Gaming;
using Core.Services.PubSub;
using Core.Services.System;
using Core.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDomain(
        this IServiceCollection services)
    {
        services.AddSingleton<CoreCacheRegistry>();
        services.AddSingleton<CorePubSubRegistry>();

        services.AddHttpClient<BinanceApi>();
        services.AddHttpClient<CoincapApi>();

        services.AddScoped<SessionService>();
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        services.AddScoped<AffiliateService>();
        services.AddScoped<BudgetService>();
        services.AddScoped<BalanceService>();
        services.AddScoped<ProfileService>();
        services.AddScoped<CurrencyRatesService>();
        services.AddScoped<GameService>();
        services.AddScoped<GameHistoryService>();
        services.AddScoped<FraudService>();

        services.AddSingleton<DomainService>();
        services.AddHostedService(sp => sp.GetRequiredService<DomainService>());

        return services;
    }
}