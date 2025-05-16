namespace Core.Contracts.Entities;

public sealed record OutboxMessageEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string Exchange { get; set; } = null!;
    public string RoutingKey { get; set; } = null!;
    public string Payload { get; set; } = null!;
}