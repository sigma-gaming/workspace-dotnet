using Common.RabbitMq;
using Core.Builder;
using Core.RabbitMq.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Core.RabbitMq;

public static class DependencyInjection
{
    public static CoreBuilder AddCoreRabbitMq(
        this CoreBuilder builder,
        IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.Bind("CoreRabbitMq", rabbitMqOptions);

        builder.Services.AddSingleton(rabbitMqOptions);
        builder.Services.AddSingleton(rabbitMqOptions.PaymentWebhooks);

        var factory = new ConnectionFactory
        {
            HostName = rabbitMqOptions.Host,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.Username,
            Password = rabbitMqOptions.Password,
            VirtualHost = rabbitMqOptions.VirtualHost,
            ClientProvidedName = AppDomain.CurrentDomain.FriendlyName,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };

        builder.Services.AddSingleton(factory);
        builder.Services.AddSingleton<RabbitMqConnection>();
        builder.Services.AddSingleton<RabbitMqPublisher>();
        builder.Services.AddHostedService<RabbitMqConnectionHostedService>();

        return builder;
    }
}