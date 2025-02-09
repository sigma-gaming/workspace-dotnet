using Common.PubSub;
using Common.PubSub.Abstractions;
using Core.Builder;
using Core.PubSub.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.PubSub;

public static class DependencyInjection
{
    public static CoreBuilder AddCorePubSub(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        var cacheOptions = new RedisPubSubOptions();
        configuration.Bind("CorePubSub", cacheOptions);

        builder.Services.AddSingleton(cacheOptions);

        var options = new ConfigurationOptions
        {
            EndPoints =
            {
                { cacheOptions.Host, cacheOptions.Port },
            },
            Password = cacheOptions.Password,
        };

        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(options));
        builder.Services.AddSingleton<IPubSubService, RedisPubSubService>();

        return builder;
    }
}