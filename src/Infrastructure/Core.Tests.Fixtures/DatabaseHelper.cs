using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Tests.Fixtures;

/// <summary>
/// Helper for common database operations in tests
/// </summary>
public class DatabaseHelper<TContext> where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> contextFactory;
    private readonly ILogger logger;

    /// <summary>
    /// Create a new database helper
    /// </summary>
    /// <param name="contextFactory">The DbContext factory</param>
    /// <param name="logger">Logger instance</param>
    public DatabaseHelper(IDbContextFactory<TContext> contextFactory, ILogger logger)
    {
        this.contextFactory = contextFactory;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new DbContext instance
    /// </summary>
    public async Task<TContext> CreateContextAsync() => 
        await contextFactory.CreateDbContextAsync();

    /// <summary>
    /// Execute a database operation within a transaction with automatic rollback
    /// </summary>
    public async Task ExecuteWithRollbackAsync(Func<TContext, Task> action)
    {
        await using var context = await CreateContextAsync();
        
        var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            await action(context);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    /// <summary>
    /// Execute a database operation that returns a result within a transaction with automatic rollback
    /// </summary>
    public async Task<TResult> ExecuteWithRollbackAsync<TResult>(Func<TContext, Task<TResult>> function)
    {
        await using var context = await CreateContextAsync();
        
        var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            return await function(context);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    /// <summary>
    /// Execute a raw SQL query
    /// </summary>
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        await using var context = await CreateContextAsync();
        return await context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    /// <summary>
    /// Execute a database operation within a transaction
    /// This uses ExecuteInTransactionAsync which follows the DbContext transaction pattern
    /// </summary>
    public async Task ExecuteInTransactionAsync(Func<TContext, Task> action)
    {
        await using var context = await CreateContextAsync();
        
        logger.LogDebug("Starting database transaction");
        
        try
        {
            await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await action(context);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
            
            logger.LogDebug("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed and was rolled back");
            throw;
        }
    }

    /// <summary>
    /// Execute a database operation that returns a result within a transaction
    /// </summary>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<TContext, Task<TResult>> function)
    {
        await using var context = await CreateContextAsync();
        
        logger.LogDebug("Starting database transaction");
        
        try
        {
            return await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    var result = await function(context);
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed and was rolled back");
            throw;
        }
    }
} 