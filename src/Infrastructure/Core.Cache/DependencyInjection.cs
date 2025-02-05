using Core.Builder;
using Core.Cache.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache;

public static class DependencyInjection
{
    public static CoreBuilder AddCoreCache(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        var cacheOptions = new RedisCacheOptions();
        configuration.Bind("CoreCache", cacheOptions);

        builder.Services.AddSingleton(cacheOptions);

        var options = new ConfigurationOptions
        {
            EndPoints =
            {
                { cacheOptions.Host, cacheOptions.Port }
            },
            Password = cacheOptions.Password,
            AbortOnConnectFail = false
        };

        var connection = ConnectionMultiplexer.Connect(options);
        builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
        connection.GetDatabase().PingAsync();

        return builder;
    }
}