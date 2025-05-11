using System.Data;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace Core.Tests.Fixtures;

/// <summary>
///     PostgreSQL fixture for integration tests with support for pg_uuidv7 extension
/// </summary>
public class CoreDbFixture : IAsyncLifetime, IDisposable
{
    private readonly bool createSchema;
    private readonly string databaseName;
    private readonly ILogger<CoreDbFixture> logger;
    private readonly PostgreSqlContainer postgresContainer;
    private readonly string schemaName;
    private bool isDisposed;

    private bool migrationsApplied;
    private Respawner? respawner;

    /// <summary>
    ///     Creates a new PostgreSQL fixture with custom database and schema names for isolation
    /// </summary>
    /// <param name="databaseName">Custom database name for isolation (optional)</param>
    /// <param name="schemaName">Schema name for isolation (optional)</param>
    /// <param name="createSchema">Whether to create a separate schema (default: false)</param>
    public CoreDbFixture(string? databaseName = null, string? schemaName = null, bool createSchema = false)
    {
        this.databaseName = databaseName ?? $"testdb_{Guid.NewGuid():N}";
        this.schemaName = schemaName ?? "public";
        this.createSchema = createSchema;

        // Configure PostgreSQL container with pg_uuidv7 extension
        postgresContainer = new PostgreSqlBuilder()
            .WithImage("risenx/cnpg-postgres-uuidv7:17.2-28")
            .WithDatabase(this.databaseName)
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .WithPrivileged(true)
            .Build();

        // Create a logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<CoreDbFixture>();
    }

    /// <summary>
    ///     Connection string for the PostgreSQL container
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    ///     Database context factory for creating DbContext instances
    /// </summary>
    public IDbContextFactory<CoreDbContext> DbContextFactory { get; private set; } = null!;

    /// <summary>
    ///     Service provider for DI
    /// </summary>
    public ServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    ///     Initialize the PostgreSQL container and set up database context
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Start container
            logger.LogInformation("Starting PostgreSQL container...");
            await postgresContainer.StartAsync();
            ConnectionString = postgresContainer.GetConnectionString();

            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Create DbContext factory
            DbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<CoreDbContext>>();

            // Set up database
            await SetupDatabaseAsync();

            // Initialize Respawner for cleaning between tests
            await InitializeRespawnerAsync();

            logger.LogInformation("PostgreSQL fixture initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize PostgreSQL fixture");
            throw;
        }
    }

    /// <summary>
    ///     Cleanup resources
    /// </summary>
    public async Task DisposeAsync()
    {
        await postgresContainer.StopAsync();
        await postgresContainer.DisposeAsync();
    }

    /// <summary>
    ///     Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Configure services including DbContext
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Add DbContext
        services.AddDbContextFactory<CoreDbContext>(options =>
        {
            options.UseNpgsql(ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);

                // Set schema if custom schema is used
                if (createSchema && schemaName != "public")
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);

                npgsqlOptions.MigrationsAssembly("Core.Migrations");
            });

            // Disable EF Core database command logging in tests
            options.UseLoggerFactory(CreateQuietLoggerFactory());
        });

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            // Suppress EF database logs
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });
    }

    /// <summary>
    ///     Creates a logger factory with database command logging disabled
    /// </summary>
    private static ILoggerFactory CreateQuietLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
            builder.AddFilter((category, level) =>
                category != "Microsoft.EntityFrameworkCore.Database.Command" ||
                level >= LogLevel.Warning));
    }

    /// <summary>
    ///     Setup database with required extensions and schema
    /// </summary>
    private async Task SetupDatabaseAsync()
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();

        try
        {
            logger.LogInformation("Setting up database...");

            // Verify connection
            if (!await context.Database.CanConnectAsync())
                throw new InvalidOperationException("Cannot connect to the database");

            // Create UUID extension
            await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS \"pg_uuidv7\";");
            logger.LogInformation("UUID extension created");

            // Create custom schema if needed
            if (createSchema && schemaName != "public")
            {
                await context.Database.ExecuteSqlAsync($"CREATE SCHEMA IF NOT EXISTS {schemaName};");
                logger.LogInformation("Schema {SchemaName} created", schemaName);
            }

            // Apply migrations
            await ApplyMigrationsAsync(context);

            logger.LogInformation("Database setup completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting up database");
            throw;
        }
    }

    /// <summary>
    ///     Apply database migrations
    /// </summary>
    protected virtual async Task ApplyMigrationsAsync(CoreDbContext context)
    {
        // Apply migrations by default
        if (migrationsApplied) return;
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        migrationsApplied = true;
        logger.LogInformation("Database migrations applied");
    }

    /// <summary>
    ///     Initialize Respawner for database cleanup between tests
    /// </summary>
    private async Task InitializeRespawnerAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Create respawner options with or without schema filtering
            RespawnerOptions options;
            if (createSchema && schemaName != "public")
                options = new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    SchemasToInclude = [schemaName],
                };
            else
                options = new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                };

            respawner = await Respawner.CreateAsync(connection, options);

            logger.LogInformation("Respawner initialized successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("No tables found"))
        {
            logger.LogWarning("No tables found for Respawner. This is expected for empty databases.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Respawner");
            throw;
        }
    }

    /// <summary>
    ///     Reset database to a clean state between tests
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        if (respawner == null)
        {
            logger.LogWarning("Respawner not initialized, cannot reset database");
            return;
        }

        try
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await respawner.ResetAsync(connection);
            logger.LogInformation("Database reset completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reset database");
            throw;
        }
    }

    /// <summary>
    ///     Create a new CoreDbContext instance
    /// </summary>
    public async Task<CoreDbContext> CreateDbContextAsync()
    {
        return await DbContextFactory.CreateDbContextAsync();
    }

    /// <summary>
    ///     Execute an action within a transaction with automatic rollback
    /// </summary>
    public async Task ExecuteWithRollbackAsync(Func<CoreDbContext, Task> action)
    {
        await using var context = await CreateDbContextAsync();
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
    ///     Dispose resources
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing) ServiceProvider?.Dispose();

        isDisposed = true;
    }

    /// <summary>
    ///     Finalizer
    /// </summary>
    ~CoreDbFixture()
    {
        Dispose(false);
    }
}