using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;
using Xunit;

namespace Core.Tests.Fixtures;

/// <summary>
/// RabbitMQ fixture for integration tests
/// </summary>
public class CoreRabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer rabbitMqContainer;
    private readonly ILogger<CoreRabbitMqFixture> logger;

    /// <summary>
    /// Connection string for RabbitMQ
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new RabbitMQ fixture
    /// </summary>
    public CoreRabbitMqFixture()
    {
        // Configure RabbitMQ container, quorum queues need RabbitMQ 3.8+, dead lettering strategies 3.10+
        rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:4.1.0-alpine")
            .WithCleanUp(true)
            .Build();

        // Create a logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<CoreRabbitMqFixture>();
    }

    /// <summary>
    /// Initialize the RabbitMQ container
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Start container
            logger.LogInformation("Starting RabbitMQ container...");
            await rabbitMqContainer.StartAsync();
            ConnectionString = rabbitMqContainer.GetConnectionString();
            logger.LogInformation("RabbitMQ fixture initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize RabbitMQ fixture");
            throw;
        }
    }

    /// <summary>
    /// Create a connection factory for the RabbitMQ container
    /// </summary>
    public ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            Uri = new Uri(ConnectionString),
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };
    }

    /// <summary>
    /// Get the number of ready messages in a queue
    /// </summary>
    public async Task<uint> GetMessageCountAsync(string queue)
    {
        await using var connection = await CreateConnectionFactory().CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        return await channel.MessageCountAsync(queue);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            await rabbitMqContainer.StopAsync();
            await rabbitMqContainer.DisposeAsync();
            logger.LogInformation("RabbitMQ fixture disposed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disposing RabbitMQ fixture");
        }
    }
}