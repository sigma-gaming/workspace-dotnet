using Microsoft.Extensions.Logging;
using NATS.Client;
using Testcontainers.Nats;
using Xunit;

namespace Core.Tests.Fixtures;

/// <summary>
/// NATS fixture for integration tests
/// </summary>
public class CoreNatsFixture : IAsyncLifetime, IDisposable
{
    private readonly NatsContainer natsContainer;
    private readonly ILogger<CoreNatsFixture> logger;
    private bool isDisposed;

    /// <summary>
    /// NATS connection for tests
    /// </summary>
    public IConnection Connection { get; private set; } = null!;

    /// <summary>
    /// Connection string for NATS
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new NATS fixture
    /// </summary>
    public CoreNatsFixture()
    {
        // Configure NATS container
        natsContainer = new NatsBuilder()
            .WithImage("nats:2.10-alpine")
            .WithCleanUp(true)
            .Build();

        // Create a logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<CoreNatsFixture>();
    }

    /// <summary>
    /// Initialize the NATS container and create connection
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Start container
            logger.LogInformation("Starting NATS container...");
            await natsContainer.StartAsync();
            ConnectionString = natsContainer.GetConnectionString();

            // Connect to NATS
            var connectionFactory = new ConnectionFactory();
            Connection = connectionFactory.CreateConnection(ConnectionString);
            logger.LogInformation("NATS fixture initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize NATS fixture");
            throw;
        }
    }

    /// <summary>
    /// Create a new NATS subscription
    /// </summary>
    public IAsyncSubscription CreateSubscription(string subject, EventHandler<MsgHandlerEventArgs> handler)
    {
        logger.LogInformation("Creating subscription for subject: {Subject}", subject);
        return Connection.SubscribeAsync(subject, handler);
    }

    /// <summary>
    /// Publish a message to a NATS subject
    /// </summary>
    public void Publish(string subject, byte[] data)
    {
        logger.LogInformation("Publishing to subject: {Subject}", subject);
        Connection.Publish(subject, data);
    }

    /// <summary>
    /// Publish a message as a string to a NATS subject
    /// </summary>
    public void Publish(string subject, string message)
    {
        Publish(subject, System.Text.Encoding.UTF8.GetBytes(message));
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;
        
        if (disposing)
        {
            Connection?.Dispose();
        }
        
        isDisposed = true;
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~CoreNatsFixture()
    {
        Dispose(false);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            // Close NATS connection
            Connection?.Dispose();
            
            // Stop container
            await natsContainer.StopAsync();
            await natsContainer.DisposeAsync();
            
            logger.LogInformation("NATS fixture disposed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disposing NATS fixture");
        }
    }
} 