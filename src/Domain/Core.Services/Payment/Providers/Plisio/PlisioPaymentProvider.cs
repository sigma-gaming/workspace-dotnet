using System.Globalization;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Core.Services.Payment.Providers.Plisio.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Plisio payment provider implementation
/// </summary>
public class PlisioPaymentProvider(
    PlisioApiClient apiClient,
    IOptions<PlisioOptions> options,
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
            var createInvoiceRequest = new PlisioCreateInvoiceRequest
            {
                Currency = MapCurrency(input.Currency),
                SourceCurrency = "USD",
                SourceAmount = input.GemAmount.ToString(CultureInfo.InvariantCulture),
                OrderName = $"Deposit {input.GemAmount} gems",
                OrderNumber = Guid.NewGuid().ToString("N"),
                Description = $"Deposit {input.GemAmount} gems",
                CallbackUrl = "https://app.sigma.local:5173",
                ExpireMin = options.InvoiceExpirationMinutes.ToString(),
            };

            var invoice = await apiClient.CreateInvoiceAsync(createInvoiceRequest);

            return new DepositResponse
            {
                ProviderTransactionId = invoice.TransactionId,
                Status = PlisioStatusMapper.MapInvoiceStatus(invoice.Status),
                ProviderAmount = invoice.Amount,
                Currency = input.Currency,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Type = DepositType.Redirect,
                Payload = new DepositPayloadRedirect
                {
                    RedirectUrl = invoice.InvoiceUrl,
                },
            };
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
            var createWithdrawalRequest = new PlisioCreateWithdrawalRequest
            {
                PsysCid = MapCurrency(input.Currency),
                To = input.DestinationAddress,
                Amount = input.GemAmount.ToString(),
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