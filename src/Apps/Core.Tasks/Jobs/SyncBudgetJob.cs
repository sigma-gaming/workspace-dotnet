using Core.Services.Finance;
using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that syncs the budget
/// </summary>
public class SyncBudgetJob(
    BudgetService budgetService,
    ILogger<SyncBudgetJob>? logger = null)
{
    /// <summary>
    ///     Executes the budget sync job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting budget sync job at: {Time}", DateTimeOffset.Now);

        try
        {
            await budgetService.SyncBudgetAsync(ct);

            logger?.LogInformation("Budget sync job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during budget sync job");
            throw;
        }
    }
}