using System.Data;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Tests to verify that PostgreSQL container setup is working correctly.
/// </summary>
[Collection("PaymentTestCollection")]
public class PostgreSqlSetupTests : IAsyncLifetime
{
    private readonly PaymentDbFixture dbFixture;
    private CoreDbContext dbContext = null!;

    public PostgreSqlSetupTests(PaymentDbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }
    
    public async Task InitializeAsync()
    {
        dbContext = await dbFixture.CreateDbContextAsync();
    }
    
    public async Task DisposeAsync()
    {
        await dbContext.DisposeAsync();
    }

    [Fact]
    public async Task PostgreSql_ShouldBeAccessible()
    {
        // This test verifies that the PostgreSQL container is accessible

        // Verify database context is available
        Assert.NotNull(dbContext);

        // Verify connection is working
        Assert.True(await dbContext.Database.CanConnectAsync());

        // Ensure we can execute a simple query
        var result = await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
        Assert.Equal(-1, result); // ExecuteSqlRawAsync returns -1 for non-query statements
    }

    [Fact]
    public async Task PostgreSql_ShouldHaveUuidv7Extension()
    {
        // This test verifies that the pg_uuidv7 extension is available and working

        // Test that we can create a connection and execute query within transaction
        await dbFixture.ExecuteWithRollbackAsync(async context =>
        {
            // Check if the extension exists using direct SQL execution
            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT EXISTS(SELECT 1 FROM pg_extension WHERE extname = 'pg_uuidv7')";
            var result = await command.ExecuteScalarAsync();
            var extensionExists = result != null && (bool)result;

            Assert.True(extensionExists, "pg_uuidv7 extension should be installed");

            // Verify that we can use the extension to generate UUIDs
            await using var cmd2 = connection.CreateCommand();
            cmd2.CommandText = "SELECT uuid_generate_v7()";
            var uuid = await cmd2.ExecuteScalarAsync();

            Assert.NotNull(uuid);
            Assert.IsType<Guid>(uuid);
        });
    }

    [Fact]
    public async Task PostgreSql_ShouldSupportTransactions()
    {
        // This test verifies that transactions work correctly using our ExecuteWithRollbackAsync method
        
        // Flag to track if we saw data inside transaction
        var foundDataInsideTransaction = false;
        
        try
        {
            await dbFixture.ExecuteWithRollbackAsync(async context =>
            {
                // Create a temporary table
                await context.Database.ExecuteSqlRawAsync(
                    "CREATE TEMPORARY TABLE IF NOT EXISTS test_transactions (id SERIAL PRIMARY KEY, value TEXT)");

                // Insert data
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO test_transactions (value) VALUES ('test_value')");

                // Verify data exists within the transaction
                var connection = context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open) await connection.OpenAsync();

                await using var countCommand = connection.CreateCommand();
                countCommand.CommandText = "SELECT COUNT(*) FROM test_transactions";
                var count = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                Assert.Equal(1, count);
                
                foundDataInsideTransaction = true;
            });
        }
        catch (Exception ex)
        {
            // We shouldn't get here since ExecuteWithRollbackAsync handles the rollback
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
        
        // Verify we saw the data inside the transaction
        Assert.True(foundDataInsideTransaction, "Should have found data inside transaction");

        // After transaction completed, verify that the table does not exist (should throw exception)
        var connectionAfter = dbContext.Database.GetDbConnection();
        if (connectionAfter.State != ConnectionState.Open) await connectionAfter.OpenAsync();
        
        await Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var afterRollbackCommand = connectionAfter.CreateCommand();
            afterRollbackCommand.CommandText = "SELECT COUNT(*) FROM test_transactions";
            await afterRollbackCommand.ExecuteScalarAsync();
        });
    }
}