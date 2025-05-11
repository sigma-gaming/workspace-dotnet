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
    InitializeChatMessagesJob initializeChatMessagesJob,
    ProcessReferrerPayoutsJob processReferrerPayoutsJob,
    ProcessStaleDepositsJob processStaleDepositsJob,
    ProcessStaleWithdrawalsJob processStaleWithdrawalsJob,
    SendAdvancedGamesMetricsJob sendAdvancedGamesMetricsJob,
    SyncBudgetJob syncBudgetJob,
    UpdateCurrencyRatesJob updateCurrencyRatesJob,
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

        // Register chat messages initialization job to run every 10 minutes
        RecurringJob.AddOrUpdate(
            "initialize-chat-messages-job",
            () => initializeChatMessagesJob.ExecuteAsync(CancellationToken.None),
            "*/10 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register referrer payouts job to run every hour
        RecurringJob.AddOrUpdate(
            "process-referrer-payouts-job",
            () => processReferrerPayoutsJob.ExecuteAsync(CancellationToken.None),
            "0 */1 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register stale deposits job to run every 10 minutes
        RecurringJob.AddOrUpdate(
            "process-stale-deposits-job",
            () => processStaleDepositsJob.ExecuteAsync(CancellationToken.None),
            "*/10 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register stale withdrawals job to run every 10 minutes
        RecurringJob.AddOrUpdate(
            "process-stale-withdrawals-job",
            () => processStaleWithdrawalsJob.ExecuteAsync(CancellationToken.None),
            "*/10 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register advanced games metrics job to run every 5 minutes
        RecurringJob.AddOrUpdate(
            "send-advanced-games-metrics-job",
            () => sendAdvancedGamesMetricsJob.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register budget sync job to run every 5 minutes
        RecurringJob.AddOrUpdate(
            "sync-budget-job",
            () => syncBudgetJob.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        // Register currency rates update job to run every 5 minutes
        RecurringJob.AddOrUpdate(
            "update-currency-rates-job",
            () => updateCurrencyRatesJob.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"),
            });

        logger?.LogInformation("Registered all recurring background jobs");
    }
}