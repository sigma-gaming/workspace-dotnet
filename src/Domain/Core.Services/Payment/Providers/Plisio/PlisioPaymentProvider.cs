using System.Globalization;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Services.CurrencyRates;
using Core.Services.Payment.Exceptions;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.Payment.Providers.Plisio.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using InvalidOperationException = System.InvalidOperationException;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Plisio payment provider implementation
/// </summary>
public class PlisioPaymentProvider(
    PlisioApiClient apiClient,
    IOptions<PlisioOptions> options,
    ICurrencyRatesService currencyRatesService,
    ILogger<PlisioPaymentProvider> logger)
    : PaymentProviderService(logger), IDepositProvider, IWithdrawalProvider
{
    private readonly PlisioOptions options = options.Value;

    /// <summary>
    ///     Gets the payment provider type
    /// </summary>
    public override PaymentProvider Provider => PaymentProvider.Plisio;

    /// <summary>
    ///     Creates a deposit request with the payment provider
    /// </summary>
    public async Task<DepositResponse> CreateDepositAsync(DepositInput input)
    {
        try
        {
            // Convert gems to USD for deposit
            decimal usdAmount;
            try
            {
                usdAmount = (decimal)await currencyRatesService.ConvertGemsAsync(input.GemAmount, Currency.Usd);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, "Failed to convert gems to USD: currency rates not available");
                throw new PaymentException("Deposits are unavailable at the moment. Please try again later.", ex);
            }

            var createInvoiceRequest = new PlisioCreateInvoiceRequest
            {
                Currency = MapCurrency(input.Currency),
                SourceCurrency = "USD",
                SourceAmount = usdAmount.ToString(CultureInfo.InvariantCulture),
                OrderName = $"Deposit {input.GemAmount} gems",
                OrderNumber = Guid.NewGuid().ToString("N"),
                Description = $"Deposit {input.GemAmount} gems",
                CallbackUrl = "https://app.sigma.local:5173",
                ExpireMin = options.InvoiceExpirationMinutes.ToString(),
            };

            // Get the minimal invoice creation response
            var createInvoiceResponse = await apiClient.CreateInvoiceAsync(createInvoiceRequest);

            // Fetch the full invoice details using the transaction ID
            var invoice = await apiClient.GetInvoiceAsync(createInvoiceResponse.TransactionId);

            return new DepositResponse
            {
                ProviderTransactionId = createInvoiceResponse.TransactionId,
                Status = PlisioStatusMapper.MapInvoiceStatus(invoice.Status),
                ProviderAmount = invoice.Amount,
                Currency = input.Currency,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Flow = DepositFlow.Redirect,
                FlowDetails = new DepositFlowDetailsRedirect
                {
                    RedirectUrl = createInvoiceResponse.InvoiceUrl,
                },
            };
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create deposit with Plisio");
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
            var invoice = await apiClient.GetInvoiceAsync(providerTransactionId);
            return PlisioStatusMapper.MapInvoiceStatus(invoice.Status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get deposit status from Plisio");
            throw;
        }
    }

    /// <summary>
    ///     Cancels a deposit with the payment provider
    /// </summary>
    public Task CancelDepositAsync(string providerTransactionId)
    {
        // Plisio doesn't have an API to cancel deposits
        Logger.LogInformation("Cancelling deposit {TransactionId} (not supported by Plisio)", providerTransactionId);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the transaction status from the payment provider
    /// </summary>
    public override async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        try
        {
            var operation = await apiClient.GetTransactionStatusAsync(transactionId);
            return PlisioStatusMapper.MapWithdrawalStatus(operation.Status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get transaction status from Plisio");
            throw;
        }
    }

    /// <summary>
    ///     Creates a withdrawal request with the payment provider
    /// </summary>
    public async Task<WithdrawalResponse> CreateWithdrawalAsync(WithdrawalInput input)
    {
        try
        {
            // Convert gems to target currency for withdrawal
            decimal currencyAmount;
            try
            {
                currencyAmount = (decimal)await currencyRatesService.ConvertGemsAsync(input.GemAmount, input.Currency);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, "Failed to convert gems to {Currency}: currency rates not available",
                    input.Currency);
                throw new PaymentException("Withdrawals are unavailable at the moment. Please try again later.", ex);
            }

            var createWithdrawalRequest = new PlisioCreateWithdrawalRequest
            {
                PsysCid = MapCurrency(input.Currency),
                To = input.DestinationAddress,
                Amount = currencyAmount.ToString(CultureInfo.InvariantCulture),
                Type = "cash_out",
                FeePlan = "normal",
            };

            var withdrawal = await apiClient.CreateWithdrawalAsync(createWithdrawalRequest);

            return new WithdrawalResponse
            {
                ProviderTransactionId = withdrawal.TransactionId,
                Status = PlisioStatusMapper.MapWithdrawalStatus(withdrawal.Status),
                ProviderAmount = withdrawal.Amount,
                Currency = input.Currency,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }
        catch (PaymentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create withdrawal with Plisio");
            throw;
        }
    }

    /// <summary>
    ///     Gets the status of a withdrawal from the payment provider
    /// </summary>
    public async Task<PaymentStatus> GetWithdrawalStatusAsync(string providerTransactionId)
    {
        try
        {
            var operation = await apiClient.GetTransactionStatusAsync(providerTransactionId);
            return PlisioStatusMapper.MapWithdrawalStatus(operation.Status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get withdrawal status from Plisio");
            throw;
        }
    }

    /// <summary>
    ///     Cancels a withdrawal with the payment provider
    /// </summary>
    public Task CancelWithdrawalAsync(string providerTransactionId)
    {
        // Plisio doesn't have an API to cancel withdrawals
        Logger.LogInformation("Cancelling withdrawal {TransactionId} (not supported by Plisio)", providerTransactionId);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Maps Currency enum to Plisio currency code
    /// </summary>
    private string MapCurrency(Currency currency)
    {
        return currency switch
        {
            Currency.Ton => "TON",
            Currency.Btc => "BTC",
            Currency.Eth => "ETH",
            Currency.UsdtTrc20 => "USDT",
            Currency.UsdtErc20 => "USDC",
            _ => "TON",
        };
    }
}