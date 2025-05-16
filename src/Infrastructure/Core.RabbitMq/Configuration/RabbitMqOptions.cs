using Common.RabbitMq;

namespace Core.RabbitMq.Configuration;

public class RabbitMqOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";

    public RabbitMqQueueOptions PaymentWebhooks { get; set; } = new()
    {
        Exchange = "payments",
        Queue = "payment-webhooks",
        RoutingKey = "payment.webhook",
    };
}