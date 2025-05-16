using System.Text.Json;
using Core.Contracts.Entities;
using Core.Db.Context;

namespace Core.Services.Outbox;

/// <summary>
///     Writes messages to the outbox table, they are published to RabbitMQ by <see cref="OutboxPublisherService" />
/// </summary>
public class OutboxService(CoreDbContext db, OutboxSignal signal)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Guid> EnqueueAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default)
    {
        var entity = new OutboxMessageEntity
        {
            Id = Guid.CreateVersion7(),
            CreatedAt = DateTime.UtcNow,
            Exchange = exchange,
            RoutingKey = routingKey,
            Payload = JsonSerializer.Serialize(message, JsonOptions),
        };

        db.OutboxMessages.Add(entity);
        await db.SaveChangesAsync(ct);

        signal.Notify();
        return entity.Id;
    }
}