using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Common.RabbitMq;

public class RabbitMqConnection(
    ConnectionFactory factory,
    IEnumerable<RabbitMqQueueOptions> queues,
    ILogger<RabbitMqConnection> logger) : IAsyncDisposable
{
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private IConnection? connection;

    public bool IsOpen => connection?.IsOpen ?? false;

    public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (connection is not null) return connection;

        await semaphore.WaitAsync(ct);
        try
        {
            if (connection is not null) return connection;

            var created = await factory.CreateConnectionAsync(ct);

            // Declared once, automatic recovery redeclares the topology after reconnects
            await using (var channel = await created.CreateChannelAsync(cancellationToken: ct))
            {
                foreach (var queue in queues)
                    await RabbitMqTopology.DeclareAsync(channel, queue, ct);
            }

            logger.LogInformation("RabbitMQ connection established to {Endpoint}", created.Endpoint);
            connection = created;
            return connection;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (connection is not null) await connection.DisposeAsync();
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}