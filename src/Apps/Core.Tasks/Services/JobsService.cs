using Core.Tasks.Jobs;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Core.Tasks.Services;

/// <summary>
///     Service that handles the registration of recurring Hangfire jobs
/// </summary>
public class JobsService(
    DataCleanupJob dataCleanupJob,
    HealthCheckJob healthCheckJob,
    ILogger<JobsService>? logger = null)
    : IJobsService
{
    /// <summary>
    ///     Registers all recurring background jobs
    /// </summary>
    public void RegisterJobs()
    {
        logger?.LogInformation("Registering recurring background jobs");

        // Register data cleanup job to run daily at 3:00 AM
        RecurringJob.AddOrUpdate(
            "data-cleanup-job",
            () => dataCleanupJob.ExecuteAsync(CancellationToken.None),
            Cron.Daily(3, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register health check job to run every 5 minutes
        RecurringJob.AddOrUpdate(
            "health-check-job",
            () => healthCheckJob.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        logger?.LogInformation("Registered all recurring background jobs");
    }
}