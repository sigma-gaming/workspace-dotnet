using Core.Builder;
using Core.Db;
using Core.Cache;
using Core.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static class DependencyInjection
{
    public static CoreBuilder AddInfrastructure(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        builder.AddCoreDb(configuration);
        builder.AddCoreCache(configuration);
        builder.AddCorePubSub(configuration);

        return builder;
    }
}