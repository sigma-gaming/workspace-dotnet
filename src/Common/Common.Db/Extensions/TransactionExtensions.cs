using Microsoft.EntityFrameworkCore;

namespace Common.Db.Extensions;

public static class TransactionExtensions
{
    public static async Task<TResult> ExecuteInTransactionAsync<TResult>(
        this DbContext context,
        Func<Task<TResult>> operation)
    {
        var existingTransaction = context.Database.CurrentTransaction;

        //  Reuse existing transaction
        if (existingTransaction is not null) return await operation();

        // Open a new transaction
        var strategy = context.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return result;
    }

    public static async Task ExecuteInTransactionAsync(
        this DbContext context,
        Func<Task> operation)
    {
        await context.ExecuteInTransactionAsync(async () =>
        {
            await operation();
            return true;
        });
    }
}