using Common.PubSub;
using Common.PubSub.Abstractions;
using Core.PubSub.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.PubSub;

public static class DependencyInjection
{
    public static IServiceCollection AddCorePubSub(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheOptions = new RedisPubSubOptions();
        configuration.Bind("CorePubSub", cacheOptions);

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
        services.AddSingleton<IPubSubService, RedisPubSubService>();

        return services;
    }
}