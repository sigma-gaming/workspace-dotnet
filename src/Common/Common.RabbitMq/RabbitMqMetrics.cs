using Prometheus;

namespace Common.RabbitMq;

public static class RabbitMqMetrics
{
    public static Counter Published { get; } =
        Metrics.CreateCounter("rabbitmq_client_published_total", "Messages confirmed by the broker", "exchange");

    public static Counter Consumed { get; } =
        Metrics.CreateCounter("rabbitmq_client_consumed_total", "Messages processed and acked", "queue");

    public static Counter Retried { get; } =
        Metrics.CreateCounter("rabbitmq_client_retried_total", "Messages sent to the retry queue", "queue");

    public static Counter DeadLettered { get; } =
        Metrics.CreateCounter("rabbitmq_client_dead_lettered_total", "Messages moved to the dead letter queue",
            "queue");
}