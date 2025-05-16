namespace Common.RabbitMq;

public class RabbitMqQueueOptions
{
    public string Exchange { get; set; } = null!;
    public string Queue { get; set; } = null!;
    public string RoutingKey { get; set; } = null!;
    public ushort PrefetchCount { get; set; } = 20;
    public ushort Concurrency { get; set; } = 4;
    public int MaxAttempts { get; set; } = 5;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    public string RetryExchange => $"{Exchange}.retry";
    public string RetryQueue => $"{Queue}.retry";
    public string DeadLetterExchange => $"{Exchange}.dead";
    public string DeadLetterQueue => $"{Queue}.dead";
}