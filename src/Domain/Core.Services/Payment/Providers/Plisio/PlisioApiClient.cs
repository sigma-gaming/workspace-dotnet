using System.Text.Json;
using Core.Services.Payment.Providers.Plisio.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.Plisio;

/// <summary>
///     Client for Plisio API
/// </summary>
public class PlisioApiClient(
    HttpClient httpClient,
    IOptions<PlisioOptions> optionsRef,
    ILogger<PlisioApiClient>? logger = null)
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly PlisioOptions options = optionsRef.Value;

    /// <summary>
    ///     Creates a new invoice
    /// </summary>
    public async Task<PlisioCreateInvoiceResponse> CreateInvoiceAsync(PlisioCreateInvoiceRequest request)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["api_key"] = options.ApiKey,
            ["currency"] = request.Currency,
            ["order_name"] = request.OrderName,
            ["order_number"] = request.OrderNumber,
        };

        if (!string.IsNullOrEmpty(request.Amount))
            queryParams["amount"] = request.Amount;

        if (!string.IsNullOrEmpty(request.SourceCurrency))
            queryParams["source_currency"] = request.SourceCurrency;

        if (!string.IsNullOrEmpty(request.SourceAmount))
            queryParams["source_amount"] = request.SourceAmount;

        if (!string.IsNullOrEmpty(request.AllowedPsysCids))
            queryParams["allowed_psys_cids"] = request.AllowedPsysCids;

        if (!string.IsNullOrEmpty(request.Description))
            queryParams["description"] = request.Description;

        if (!string.IsNullOrEmpty(request.CallbackUrl))
            queryParams["callback_url"] = request.CallbackUrl;

        if (!string.IsNullOrEmpty(request.ExpireMin))
            queryParams["expire_min"] = request.ExpireMin;

        var response = await SendRequestAsync<PlisioCreateInvoiceResponse>("invoices/new", queryParams);
        return response;
    }

    /// <summary>
    ///     Gets invoice details
    /// </summary>
    public async Task<PlisioInvoiceResponse> GetInvoiceAsync(string invoiceId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["api_key"] = options.ApiKey,
        };

        var response = await SendRequestAsync<PlisioInvoiceDetailsResponse>($"invoices/{invoiceId}", queryParams);
        return response.Invoice;
    }

    /// <summary>
    ///     Creates a withdrawal
    /// </summary>
    public async Task<PlisioWithdrawalResponse> CreateWithdrawalAsync(PlisioCreateWithdrawalRequest request)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["api_key"] = options.ApiKey,
            ["psys_cid"] = request.PsysCid,
            ["to"] = request.To,
            ["amount"] = request.Amount,
            ["type"] = request.Type,
        };

        if (!string.IsNullOrEmpty(request.FeePlan))
            queryParams["feePlan"] = request.FeePlan;

        var response = await SendRequestAsync<PlisioWithdrawalResponse>("operations/withdraw", queryParams);
        return response;
    }

    /// <summary>
    ///     Gets transaction status
    /// </summary>
    public async Task<PlisioOperationResponse> GetTransactionStatusAsync(string transactionId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["api_key"] = options.ApiKey,
        };

        var response = await SendRequestAsync<PlisioOperationResponse>($"operations/{transactionId}", queryParams);
        return response;
    }

    /// <summary>
    ///     Gets supported cryptocurrencies
    /// </summary>
    public async Task<List<PlisioCryptocurrencyResponse>> GetCryptocurrenciesAsync(string fiatCurrency = "USD")
    {
        var queryParams = new Dictionary<string, string>
        {
            ["api_key"] = options.ApiKey,
        };

        var response =
            await SendRequestAsync<List<PlisioCryptocurrencyResponse>>($"currencies/{fiatCurrency}", queryParams);
        return response;
    }

    private async Task<T> SendRequestAsync<T>(string endpoint, Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        var url = $"https://plisio.net/api/v1/{endpoint}?{queryString}";
        logger?.LogInformation("Calling Plisio API: {Endpoint}", url);

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PlisioSuccessResponse<T>>(content, jsonOptions);

            if (result == null || result.Status != "success")
            {
                var errorResult = JsonSerializer.Deserialize<PlisioErrorResponse>(content, jsonOptions);
                throw new Exception($"Plisio API error: {errorResult?.Data.Message ?? "Unknown error"}");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error calling Plisio API: {Endpoint}", endpoint);
            throw;
        }
    }
}