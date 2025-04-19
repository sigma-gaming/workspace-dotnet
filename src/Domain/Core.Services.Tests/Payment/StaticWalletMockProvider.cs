using Core.Contracts.Enums;
using Core.Services.Payment.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Mock implementation of IStaticWalletProvider for testing purposes
/// </summary>
public class StaticWalletMockProvider(ILogger<StaticWalletMockProvider>? logger = null)
    : IStaticWalletProvider, ITestPaymentProvider
{
    private readonly Dictionary<string, string> generatedWallets = new();

    private readonly Dictionary<Currency, bool> supportedCurrencies = new()
    {
        { Currency.Ton, true },
        { Currency.Btc, true },
        { Currency.Eth, true },
        { Currency.UsdtTrc20, true },
        { Currency.UsdtErc20, false }, // Not supported
    };

    public PaymentProvider Provider => PaymentProvider.CryptoCloud;

    public Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        return Task.FromResult(PaymentStatus.Completed);
    }

    public bool SupportsStaticWallets(Currency currency)
    {
        return supportedCurrencies.TryGetValue(currency, out var isSupported) && isSupported;
    }

    public Task<string> GenerateWalletAddressAsync(Currency currency, Guid userId)
    {
        if (!SupportsStaticWallets(currency))
            throw new Core.Services.Payment.Exceptions.UnsupportedOperationException(
                Provider, 
                $"Currency {currency} not supported for static wallets");

        var key = $"{userId}_{currency}";

        // If we already generated a wallet for this user and currency, return it
        if (generatedWallets.TryGetValue(key, out var existingAddress))
            return Task.FromResult(existingAddress);

        // Generate a mock wallet address based on currency
        var prefix = currency switch
        {
            Currency.Ton => "UQ",
            Currency.Btc => "bc1",
            Currency.Eth => "0x",
            Currency.UsdtTrc20 => "T",
            _ => "addr",
        };

        var walletAddress = $"{prefix}{Guid.NewGuid().ToString("N").Substring(0, 24)}";

        // Store the generated address
        generatedWallets[key] = walletAddress;

        logger?.LogInformation("Generated mock wallet address {Address} for user {UserId} and currency {Currency}",
            walletAddress, userId, currency);

        return Task.FromResult(walletAddress);
    }

    public string TestIdentifier => "StaticWalletTest";

    public bool SupportsCurrency(Currency currency) => 
        supportedCurrencies.TryGetValue(currency, out var supported) && supported;
}