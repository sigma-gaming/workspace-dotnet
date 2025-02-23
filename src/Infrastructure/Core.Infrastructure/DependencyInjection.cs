using Core.Builder;
using Core.Cache;
using Core.Db;
using Core.Nats;
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
        builder.AddNats(configuration);

        return builder;
    }
}