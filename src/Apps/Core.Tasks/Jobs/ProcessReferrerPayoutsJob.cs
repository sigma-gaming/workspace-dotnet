using Core.Services.Promotions;
using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that processes referrer payouts
/// </summary>
public class ProcessReferrerPayoutsJob(
    AffiliateService affiliateService,
    ILogger<ProcessReferrerPayoutsJob>? logger = null)
{
    /// <summary>
    ///     Executes the referrer payouts processing job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Starting referrer payouts processing job at: {Time}", DateTimeOffset.Now);

        try
        {
            await affiliateService.ProcessReferrerPayoutsAsync(cancellationToken);
            
            logger?.LogInformation("Referrer payouts processing job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during referrer payouts processing job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}
