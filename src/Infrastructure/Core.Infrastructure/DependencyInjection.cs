using Core.Database;
using Core.Cache;
using Core.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCoreDatabase(configuration);
        services.AddCoreCache(configuration);
        services.AddCorePubSub(configuration);

        return services;
    }
}