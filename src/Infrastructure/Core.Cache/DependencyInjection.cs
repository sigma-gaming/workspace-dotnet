using Core.Cache.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheOptions = new RedisCacheOptions();
        configuration.Bind("CoreCache", cacheOptions);

        services.AddSingleton(cacheOptions);

        var options = new ConfigurationOptions
        {
            EndPoints =
            {
                { cacheOptions.Host, cacheOptions.Port }
            },
            Password = cacheOptions.Password,
        };

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(options));

        return services;
    }
}