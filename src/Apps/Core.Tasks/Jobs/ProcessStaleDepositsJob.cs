using Core.Services.Payment.Interfaces;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that processes stale deposits
/// </summary>
public class ProcessStaleDepositsJob(
    IPaymentService paymentService,
    ILogger<ProcessStaleDepositsJob>? logger = null)
{
    /// <summary>
    ///     Executes the stale deposits processing job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting stale deposits processing job at: {Time}", DateTimeOffset.Now);

        try
        {
            await paymentService.ProcessStaleDepositsAsync(ct);

            logger?.LogInformation("Stale deposits processing job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during stale deposits processing job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}