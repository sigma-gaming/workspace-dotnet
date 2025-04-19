using Xunit;

namespace Core.Services.Tests.Payment;

[CollectionDefinition("PaymentTestCollection")]
public class PaymentTestCollection : ICollectionFixture<PaymentDbFixture>, ICollectionFixture<PaymentCacheFixture>, ICollectionFixture<PaymentNatsFixture>
{
    // This class has no code, and is never created.
    // Its purpose is to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
} 