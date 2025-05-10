using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that performs system health checks
/// </summary>
public class HealthCheckJob(ILogger<HealthCheckJob>? logger = null)
{
    /// <summary>
    ///     Executes the health check job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Starting health check job at: {Time}", DateTimeOffset.Now);

        try
        {
            // Simulate checking various services
            await CheckDatabaseConnectionAsync(cancellationToken);
            await CheckExternalServicesAsync(cancellationToken);

            logger?.LogInformation("Health check job completed successfully - all systems operational");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during health check job - system may require attention");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }

    private async Task CheckDatabaseConnectionAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Checking database connection...");
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        // Example implementation:
        // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        // await connection.OpenAsync(cancellationToken);
        logger?.LogInformation("Database connection check successful");
    }

    private async Task CheckExternalServicesAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Checking external services...");
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        // Example implementation:
        // using var client = new HttpClient();
        // var response = await client.GetAsync("https://api.example.com/health", cancellationToken);
        // response.EnsureSuccessStatusCode();
        logger?.LogInformation("External services check successful");
    }
}