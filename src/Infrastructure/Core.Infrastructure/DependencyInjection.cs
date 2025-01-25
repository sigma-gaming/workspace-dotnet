using Core.Db;
using Core.Cache;
using Core.PubSub;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCoreDb(configuration,
            (options) => { options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); });

        services.AddCoreCache(configuration);
        services.AddCorePubSub(configuration);

        return services;
    }
}