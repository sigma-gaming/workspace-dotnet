# Core.Tests.Fixtures

This library provides a set of fixtures for integration testing with external dependencies:

- PostgreSQL with `pg_uuidv7` extension support
- Redis
- NATS

These fixtures use TestContainers to create isolated containers for your tests and follow the `IClassFixture` pattern from xUnit to maintain isolation between test classes.

## Getting Started

### 1. Reference the package

Add a reference to `Core.Tests.Fixtures` in your test projects.

### 2. Create a test class using IClassFixture

```csharp
using Core.Tests.Fixtures;
using Xunit;

public class MyPostgresTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture fixture;

    public MyPostgresTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task ShouldConnectToDatabase()
    {
        // Use the fixture
        await using var context = await fixture.CreateDbContextAsync();
        
        // Test with the context
        var canConnect = await context.Database.CanConnectAsync();
        Assert.True(canConnect);
    }
}
```

### 3. Use the IntegrationTestBase for multiple fixtures

```csharp
using Core.Tests.Fixtures;
using Xunit;

public class MyIntegrationTests : IntegrationTestBase
{
    public MyIntegrationTests(
        PostgreSqlFixture postgresFixture, 
        RedisFixture redisFixture, 
        NatsFixture natsFixture) 
        : base(postgresFixture, redisFixture, natsFixture)
    {
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Add your test services
        services.AddScoped<MyService>();
    }

    [Fact]
    public async Task TestWithAllDependencies()
    {
        // Get your service
        var myService = GetRequiredService<MyService>();
        
        // Test with isolated containers
        // ...
    }
}
```

## Fixture Features

### PostgreSqlFixture

- Creates a PostgreSQL container with `pg_uuidv7` extension
- Provides `DbContextFactory<T>` for creating context instances
- Manages transaction isolation with `ExecuteWithRollbackAsync` and `ExecuteInTransactionAsync`
- Supports database reset between tests

```csharp
// Create with custom settings
var fixture = new PostgreSqlFixture(
    databaseName: "custom_db", 
    schemaName: "custom_schema", 
    createSchema: true);

// Execute with automatic rollback
await fixture.ExecuteWithRollbackAsync(async context => {
    // Database operations that will be rolled back
});

// Create a context
var context = await fixture.CreateDbContextAsync();
```

### RedisFixture

- Creates a Redis container
- Provides key isolation with prefixes
- Supports clearing test keys between tests

```csharp
// Get a database with isolation
var db = fixture.GetDatabase();

// Generate an isolated key
var key = fixture.GetPrefixedKey("my-key");
await db.StringSetAsync(key, "test-value");

// Clear test keys
await fixture.ClearTestKeysAsync();
```

### NatsFixture

- Creates a NATS container
- Provides subject isolation with prefixes
- Simplifies subscription and publishing

```csharp
// Publish with isolation
fixture.Publish("my-subject", "test-message");

// Subscribe with isolation
var subscription = fixture.CreateSubscription("my-subject", (sender, args) => {
    // Handle message
});
```

## Transaction Management

The fixtures provide utilities for transaction management, following best practices:

```csharp
// Using DatabaseHelper with ExecuteInTransactionAsync
var helper = CreateDatabaseHelper<MyDbContext>();
await helper.ExecuteInTransactionAsync(async context => {
    // Your database operations
    await context.SaveChangesAsync();
});
```

## Customization

Each fixture can be customized by inheriting and overriding methods:

```csharp
public class MyCustomPostgresFixture : PostgreSqlFixture
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        // Add custom services
    }
    
    protected override async Task ApplyMigrationsAsync(CoreDbContext context)
    {
        // Custom migration logic
    }
}
``` 