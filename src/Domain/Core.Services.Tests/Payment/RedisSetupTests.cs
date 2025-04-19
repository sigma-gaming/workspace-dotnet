using Xunit;

namespace Core.Services.Tests.Payment;

[Collection("PaymentTestCollection")]
public class RedisSetupTests
{
    private readonly PaymentCacheFixture cacheFixture;

    public RedisSetupTests(PaymentCacheFixture cacheFixture)
    {
        this.cacheFixture = cacheFixture;
    }

    [Fact]
    public void Redis_Should_BeConnected()
    {
        // Assert
        Assert.NotNull(cacheFixture.Connection);
        Assert.True(cacheFixture.Connection.IsConnected);
    }

    [Fact]
    public async Task Redis_Should_StoreAndRetrieveValue()
    {
        // Arrange
        var db = cacheFixture.GetDatabase();
        const string key = "test:key";
        const string value = "test-value";

        // Act
        await db.StringSetAsync(key, value);
        var retrieved = await db.StringGetAsync(key);

        // Clean up
        await db.KeyDeleteAsync(key);

        // Assert
        Assert.Equal(value, retrieved.ToString());
    }
}