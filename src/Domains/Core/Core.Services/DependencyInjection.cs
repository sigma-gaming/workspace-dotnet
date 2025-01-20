using Core.Services.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDomain(
        this IServiceCollection services)
    {
        services.AddSingleton<CoreCacheRegistry>();

        return services;
    }
}