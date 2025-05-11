using Core.Services.Chat;
using Microsoft.Extensions.Logging;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that initializes chat messages
/// </summary>
public class InitializeChatMessagesJob(
    ChatService chatService,
    ILogger<InitializeChatMessagesJob>? logger = null)
{
    /// <summary>
    ///     Executes the chat message initialization job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting chat message initialization job at: {Time}", DateTimeOffset.Now);

        try
        {
            await chatService.InitializeMessagesAsync(ct);

            logger?.LogInformation("Chat message initialization job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during chat message initialization job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }
}