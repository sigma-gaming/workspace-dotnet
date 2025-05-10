using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that cleans up old data
/// </summary>
public class DataCleanupJob(ILogger<DataCleanupJob>? logger = null)
{
    /// <summary>
    ///     Executes the data cleanup job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Starting data cleanup job at: {Time}", DateTimeOffset.Now);

        try
        {
            // Simulate some cleanup work
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            // Example of database cleanup logic:
            // await _dbContext.DeleteOldLogsAsync(DateTime.Now.AddMonths(-3), cancellationToken);
            // await _dbContext.DeleteExpiredSessionsAsync(cancellationToken);

            logger?.LogInformation("Data cleanup job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during data cleanup job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}