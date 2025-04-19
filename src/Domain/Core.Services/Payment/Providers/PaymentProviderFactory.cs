using Core.Contracts.Enums;
using Core.Services.Payment.Interfaces;

namespace Core.Services.Payment.Providers;

/// <summary>
///     Factory for getting payment provider services
/// </summary>
public class PaymentProviderFactory(
    IEnumerable<IPaymentProviderService> providerServices)
{
    protected readonly Dictionary<PaymentProvider, IPaymentProviderService> Providers =
        providerServices
            .GroupBy(p => p.Provider)
            .ToDictionary(
                g => g.Key,
                g => g.Last()); // Take the last registered provider for each provider type

    /// <summary>
    ///     Gets a payment provider service by provider type
    /// </summary>
    /// <param name="provider">Payment provider type</param>
    /// <returns>Payment provider service or null if not found</returns>
    public IPaymentProviderService? GetPaymentProvider(PaymentProvider provider)
    {
        return Providers.TryGetValue(provider, out var service) ? service : null;
    }

    /// <summary>
    ///     Gets a static wallet provider by provider type
    /// </summary>
    /// <param name="provider">Payment provider type</param>
    /// <returns>Static wallet provider or null if not found or not supported</returns>
    public IStaticWalletProvider? GetStaticWalletProvider(PaymentProvider provider)
    {
        if (Providers.TryGetValue(provider, out var service) && service is IStaticWalletProvider staticWalletProvider)
            return staticWalletProvider;

        return null;
    }

    /// <summary>
    ///     Gets a deposit provider by provider type
    /// </summary>
    /// <param name="provider">Payment provider type</param>
    /// <returns>Deposit provider or null if not found</returns>
    public IDepositProvider? GetDepositProvider(PaymentProvider provider)
    {
        if (Providers.TryGetValue(provider, out var service) && service is IDepositProvider depositProvider)
            return depositProvider;

        return null;
    }

    /// <summary>
    ///     Gets a withdrawal provider by provider type
    /// </summary>
    /// <param name="provider">Payment provider type</param>
    /// <returns>Withdrawal provider or null if not found</returns>
    public IWithdrawalProvider? GetWithdrawalProvider(PaymentProvider provider)
    {
        if (Providers.TryGetValue(provider, out var service) && service is IWithdrawalProvider withdrawalProvider)
            return withdrawalProvider;

        return null;
    }
}