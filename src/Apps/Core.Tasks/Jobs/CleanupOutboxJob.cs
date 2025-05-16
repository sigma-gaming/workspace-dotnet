using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that deletes outbox messages published more than a week ago
/// </summary>
public class CleanupOutboxJob(
    CoreDbContext db,
    ILogger<CleanupOutboxJob>? logger = null)
{
    /// <summary>
    ///     Executes the outbox cleanup job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting outbox cleanup job at: {Time}", DateTimeOffset.Now);

        try
        {
            var publishedBefore = DateTime.UtcNow.AddDays(-7);

            var deleted = await db.OutboxMessages
                .Where(m => m.PublishedAt != null && m.PublishedAt < publishedBefore)
                .ExecuteDeleteAsync(ct);

            logger?.LogInformation("Outbox cleanup job deleted {Count} messages", deleted);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during outbox cleanup job");
            throw;
        }
    }
}