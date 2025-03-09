using Common.Nats;
using Core.Builder;
using Core.Nats.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;

namespace Core.Nats;

public static class DependencyInjection
{
    public static CoreBuilder AddCoreNats(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        var natsOptions = new NatsOptions();
        configuration.Bind("CoreNats", natsOptions);

        builder.Services.AddSingleton(natsOptions);

        var options = new NatsOpts
        {
            Url = natsOptions.Url,
        };

        var connection = new NatsConnection(options);
        builder.Services.AddSingleton<INatsConnection>(connection);
        builder.Services.AddHostedService<NatsConnectionHostedService>();

        return builder;
    }
}