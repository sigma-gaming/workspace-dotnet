using Core.Contracts.Entities;
using Core.Db.Context;
using Core.Services.Payment;
using Core.Services.Payment.Interfaces;
using Core.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Services.Tests.Payment;

[Collection("PaymentTestCollection")]
public class PaymentServiceIntegrationTests : PaymentIntegrationTestBase
{
    private IPaymentService? paymentService;
    private PaymentService? realPaymentService;
    private CoreDbContext dbContext = null!;

    public PaymentServiceIntegrationTests(
        PaymentDbFixture dbFixture,
        PaymentCacheFixture cacheFixture,
        PaymentNatsFixture natsFixture) 
        : base(dbFixture, cacheFixture, natsFixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        paymentService = GetService<IPaymentService>();
        realPaymentService = GetService<PaymentService>();
        dbContext = await CreateDbContextAsync();

        // Both service implementations should be available
        Assert.NotNull(paymentService);
        Assert.NotNull(realPaymentService);
    }

    [Fact]
    public async Task Should_WriteToRedis_And_WriteToPostgres()
    {
        // Redis
        var db = CacheFixture.GetDatabase();
        var redisKey = "test:integration:key";
        var redisValue = "integration-value-123";
        await db.StringSetAsync(redisKey, redisValue);
        var valueFromRedis = await db.StringGetAsync(redisKey);
        Assert.True(valueFromRedis.HasValue);
        Assert.Equal(redisValue, valueFromRedis);

        // Postgres - use a different entity type to avoid duplicate key issues
        var testEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await ExecuteWithRollbackAsync(async context => 
        {
            context.Users.Add(testEntity);
            await context.SaveChangesAsync();
            
            var entityFromDb = await context.Users.FirstOrDefaultAsync(x => x.Id == testEntity.Id);
            Assert.NotNull(entityFromDb);
            Assert.Equal(testEntity.Id, entityFromDb.Id);
        });
    }
}