namespace Core.Services.Outbox;

/// <summary>
///     Wakes up the outbox publisher right after a message is written
/// </summary>
public class OutboxSignal
{
    private readonly SemaphoreSlim semaphore = new(0, 1);

    public void Notify()
    {
        try
        {
            semaphore.Release();
        }
        catch (SemaphoreFullException)
        {
        }
    }

    public async Task WaitAsync(TimeSpan timeout, CancellationToken ct)
    {
        await semaphore.WaitAsync(timeout, ct);
    }
}