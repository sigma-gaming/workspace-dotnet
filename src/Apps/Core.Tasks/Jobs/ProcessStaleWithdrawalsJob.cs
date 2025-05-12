using Core.Services.Payment.Interfaces;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that processes stale withdrawals
/// </summary>
public class ProcessStaleWithdrawalsJob(
    IPaymentService paymentService,
    ILogger<ProcessStaleWithdrawalsJob>? logger = null)
{
    /// <summary>
    ///     Executes the stale withdrawals processing job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation("Starting stale withdrawals processing job at: {Time}", DateTimeOffset.Now);

        try
        {
            await paymentService.ProcessStaleWithdrawalsAsync(cancellationToken);

            logger?.LogInformation("Stale withdrawals processing job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during stale withdrawals processing job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}