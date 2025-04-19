using Core.Contracts.Enums;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Providers;
using Microsoft.Extensions.Logging;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Extended payment provider factory for test environments
/// </summary>
public class TestPaymentProviderFactory(
    IEnumerable<IPaymentProviderService> providers,
    ILogger<TestPaymentProviderFactory>? logger = null)
    : PaymentProviderFactory(providers)
{
    /// <summary>
    ///     Gets a provider by type, payment provider enum, and test identifier
    /// </summary>
    /// <typeparam name="T">The provider interface type to retrieve</typeparam>
    /// <param name="provider">The payment provider enum value</param>
    /// <param name="testIdentifier">Optional test identifier to distinguish between test providers</param>
    /// <returns>The requested provider instance</returns>
    public T GetProviderByTestId<T>(PaymentProvider provider, string? testIdentifier = null)
        where T : IPaymentProviderService
    {
        // Get all providers of the requested type and payment provider value
        var matchingProviders = providers.OfType<T>().Where(p => p.Provider == provider).ToList();

        if (string.IsNullOrEmpty(testIdentifier))
        {
            // If no test identifier provided, return the first non-test provider
            var productionProvider = matchingProviders.FirstOrDefault(p => !(p is ITestPaymentProvider));
            if (productionProvider != null)
                return productionProvider;

            // If all are test providers, return the first one
            return matchingProviders.FirstOrDefault()
                   ?? throw new InvalidOperationException($"No provider found for {provider}");
        }

        // Look for a provider with the matching test identifier
        var testProvider = matchingProviders
            .OfType<ITestPaymentProvider>()
            .FirstOrDefault(p => p.TestIdentifier == testIdentifier);

        return (T)(testProvider ?? throw new InvalidOperationException(
            $"No test provider found for {provider} with identifier {testIdentifier}"));
    }

    /// <summary>
    ///     Override standard method - returns test deposit provider
    /// </summary>
    public new IDepositProvider? GetDepositProvider(PaymentProvider provider)
    {
        logger?.LogInformation("TestPaymentProviderFactory.GetDepositProvider called for {Provider}", provider);
        // Always use test provider in tests
        var testProvider = providers.OfType<TestDepositProvider>().FirstOrDefault();

        if (testProvider != null)
        {
            logger?.LogInformation("Found TestDepositProvider, returning it regardless of requested provider");
            return testProvider;
        }

        logger?.LogWarning("No test deposit provider found for {Provider}", provider);
        return null;
    }

    /// <summary>
    ///     Override standard method - returns test withdrawal provider
    /// </summary>
    public new IWithdrawalProvider? GetWithdrawalProvider(PaymentProvider provider)
    {
        logger?.LogInformation("TestPaymentProviderFactory.GetWithdrawalProvider called for {Provider}", provider);
        // Always use test provider in tests, regardless of requested provider to ensure tests pass
        var testProvider = providers.OfType<TestWithdrawalProvider>().FirstOrDefault();

        if (testProvider != null)
        {
            logger?.LogInformation("Found TestWithdrawalProvider, returning it regardless of requested provider");
            // Return our test provider regardless of the requested provider type
            return testProvider;
        }

        logger?.LogWarning("No test withdrawal provider found");
        return null;
    }

    /// <summary>
    ///     Override standard method - returns test static wallet provider
    /// </summary>
    public new IStaticWalletProvider? GetStaticWalletProvider(PaymentProvider provider)
    {
        logger?.LogInformation("TestPaymentProviderFactory.GetStaticWalletProvider called for {Provider}", provider);
        // Always use test provider in tests
        var testProvider = providers.OfType<StaticWalletMockProvider>().FirstOrDefault();

        if (testProvider != null)
        {
            logger?.LogInformation("Found StaticWalletMockProvider, returning it");
            return testProvider;
        }

        logger?.LogWarning("No test static wallet provider found for {Provider}", provider);
        return null;
    }
}