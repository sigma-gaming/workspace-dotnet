using Core.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Tests.Payment;

/// <summary>
/// Database fixture for payment service tests
/// </summary>
public class PaymentDbFixture : CoreDbFixture
{
    // Static flag to track if migrations have been applied
    private static bool migrationsApplied = false;
    private readonly ILogger<PaymentDbFixture> logger;

    /// <summary>
    /// Create a new DB fixture with payment-specific configuration
    /// </summary>
    public PaymentDbFixture() : base("payment_test_db", "public", true)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<PaymentDbFixture>();
    }

    /// <summary>
    /// Override to apply migrations only once per test run
    /// </summary>
    protected override async Task ApplyMigrationsAsync(Core.Db.Context.CoreDbContext context)
    {
        if (!migrationsApplied)
        {
            logger.LogInformation("Applying database migrations for the first time...");
            await context.Database.MigrateAsync();
            migrationsApplied = true;
            logger.LogInformation("Database migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Migrations already applied, skipping...");
        }
    }
} 