using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.PubSub;
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
        services.AddScoped<BudgetService>();

        return services;
    }
}