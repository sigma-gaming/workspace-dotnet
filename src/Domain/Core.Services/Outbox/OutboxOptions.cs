namespace Core.Services.Outbox;

public class OutboxOptions
{
    public int BatchSize { get; set; } = 100;
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(1);
}