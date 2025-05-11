using Core.Services.Finance;
using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that updates currency rates
/// </summary>
public class UpdateCurrencyRatesJob(
    CurrencyRatesService currencyRatesService,
    ILogger<UpdateCurrencyRatesJob>? logger = null)
{
    /// <summary>
    ///     Executes the currency rates update job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting currency rates update job at: {Time}", DateTimeOffset.Now);

        try
        {
            await currencyRatesService.UpdateRatesAsync(ct);

            logger?.LogInformation("Currency rates update job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during currency rates update job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}