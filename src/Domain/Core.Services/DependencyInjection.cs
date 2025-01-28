using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.PubSub;
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

        services.AddScoped<SessionService>();
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        services.AddScoped<AffiliateService>();
        services.AddScoped<BudgetService>();
        services.AddScoped<BalanceService>();

        return services;
    }
}