using System.Globalization;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Services.CurrencyRates;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.Payment.Providers.CryptoCloud.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
///     CryptoCloud payment provider implementation
/// </summary>
public class CryptoCloudPaymentProvider : PaymentProviderService, IDepositProvider, IStaticWalletProvider
{
    private readonly CryptoCloudApiClient apiClient;
    private readonly ICurrencyRatesService currencyRatesService;
    private readonly CryptoCloudOptions options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CryptoCloudPaymentProvider" /> class
    /// </summary>
    public CryptoCloudPaymentProvider(
        CryptoCloudApiClient apiClient,
        IOptions<CryptoCloudOptions> options,
        ILogger<CryptoCloudPaymentProvider> logger,
        ICurrencyRatesService currencyRatesService) : base(logger)
    {
        this.apiClient = apiClient;
        this.options = options.Value;
        this.currencyRatesService = currencyRatesService;
    }

    /// <summary>
    ///     Gets the payment provider type
    /// </summary>
    public override PaymentProvider Provider => PaymentProvider.CryptoCloud;

    /// <summary>
    ///     Creates a deposit request with the payment provider
    /// </summary>
    public async Task<DepositResponse> CreateDepositAsync(DepositInput input)
    {
        try
        {
            var walletRequest = new CryptoCloudCreateWalletRequest
            {
                Currency = MapCurrency(input.Currency),
                CallbackUrl = "https://app.sigma.local:5173",
                Label = $"Deposit {input.GemAmount} gems",
            };

            var wallet = await apiClient.CreateWalletAsync(walletRequest);

            // Convert gems to target currency using CurrencyRatesService
            var currencyAmount = (decimal)await currencyRatesService.ConvertGemsAsync(input.GemAmount, input.Currency);

            return new DepositResponse
            {
                ProviderTransactionId = wallet.Id,
                Status = PaymentStatus.Pending,
                ProviderAmount = currencyAmount.ToString(CultureInfo.InvariantCulture),
                Currency = input.Currency,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Flow = DepositFlow.WhiteLabel,
                FlowDetails = new DepositFlowDetailsWhiteLabel
                {
                    Instructions = new DepositInstructionsCryptoWallet
                    {
                        Address = wallet.Address,
                        Amount = currencyAmount,
                        Currency = input.Currency,
                    },
                },
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create deposit with CryptoCloud");
            throw;
        }
    }

    /// <summary>
    ///     Gets the status of a deposit from the payment provider
    /// </summary>
    public async Task<PaymentStatus> GetDepositStatusAsync(string providerTransactionId)
    {
        try
        {
            var walletInfo = await apiClient.GetWalletInfoAsync(providerTransactionId);
            return CryptoCloudStatusMapper.MapWalletStatus(walletInfo.Status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get deposit status from CryptoCloud");
            throw;
        }
    }

    /// <summary>
    ///     Cancels a deposit with the payment provider
    /// </summary>
    public Task CancelDepositAsync(string providerTransactionId)
    {
        // CryptoCloud doesn't have an API to cancel deposits
        Logger.LogInformation("Cancelling deposit {TransactionId} (not supported by CryptoCloud)",
            providerTransactionId);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the transaction status from the payment provider
    /// </summary>
    public override async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        try
        {
            var walletInfo = await apiClient.GetWalletInfoAsync(transactionId);
            return CryptoCloudStatusMapper.MapWalletStatus(walletInfo.Status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get transaction status from CryptoCloud");
            throw;
        }
    }

    /// <summary>
    ///     Checks if the provider supports static wallets for the specified currency
    /// </summary>
    public bool SupportsStaticWallets(Currency currency)
    {
        // List of supported currencies for static wallets in CryptoCloud
        return currency is Currency.Btc or Currency.Eth or Currency.Ton or Currency.UsdtTrc20;
    }

    /// <summary>
    ///     Generates a new static wallet address for the user
    /// </summary>
    public async Task<string> GenerateWalletAddressAsync(Currency currency, Guid userId)
    {
        try
        {
            var walletRequest = new CryptoCloudCreateWalletRequest
            {
                Currency = MapCurrency(currency),
                CallbackUrl = "https://app.sigma.local:5173",
                Label = $"User {userId} wallet for {currency}",
            };

            var wallet = await apiClient.CreateWalletAsync(walletRequest);
            return wallet.Address;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate wallet address for currency {Currency}", currency);
            throw;
        }
    }

    /// <summary>
    ///     Maps Currency enum to CryptoCloud currency code
    /// </summary>
    private string MapCurrency(Currency currency)
    {
        return currency switch
        {
            Currency.Ton => "TON",
            Currency.Btc => "BTC",
            Currency.Eth => "ETH",
            Currency.UsdtTrc20 => "USDT",
            _ => throw new NotSupportedException($"Currency {currency} is not supported by CryptoCloud"),
        };
    }
}