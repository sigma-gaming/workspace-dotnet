using Core.Domain.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDomain(
        this IServiceCollection services)
    {
        services.AddSingleton<CoreCacheRegistry>();

        return services;
    }
}