using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Core.Services.Payment.Providers.CryptoCloud.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Payment.Providers.CryptoCloud;

/// <summary>
/// Client for CryptoCloud API
/// </summary>
public class CryptoCloudApiClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly ILogger<CryptoCloudApiClient> logger;
    private readonly CryptoCloudOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoCloudApiClient"/> class
    /// </summary>
    public CryptoCloudApiClient(
        HttpClient httpClient,
        IOptions<CryptoCloudOptions> options,
        ILogger<CryptoCloudApiClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options.Value;
        this.logger = logger;

        jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        
        // Set base address and default headers
        this.httpClient.BaseAddress = new Uri(this.options.ApiUrl);
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", this.options.ApiKey);
    }

    /// <summary>
    /// Creates a new wallet
    /// </summary>
    public async Task<CryptoCloudWalletData> CreateWalletAsync(CryptoCloudCreateWalletRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/v2/wallet/create/", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CryptoCloudCreateWalletResponse>(jsonOptions);
            
            if (result?.Status != "success" || result.Data == null)
            {
                throw new Exception($"Failed to create wallet: {result?.Message ?? "Unknown error"}");
            }
            
            return result.Data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create wallet in CryptoCloud");
            throw;
        }
    }

    /// <summary>
    /// Gets wallet information
    /// </summary>
    public async Task<CryptoCloudWalletInfo> GetWalletInfoAsync(string walletId)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/v2/wallet/info/{walletId}/");
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CryptoCloudWalletInfoResponse>(jsonOptions);
            
            if (result?.Status != "success" || result.Data == null)
            {
                throw new Exception($"Failed to get wallet info: {result?.Message ?? "Unknown error"}");
            }
            
            return result.Data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallet info from CryptoCloud");
            throw;
        }
    }

    /// <summary>
    /// Gets wallet address
    /// </summary>
    public async Task<string> GetWalletAddressAsync(string walletId)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/v2/wallet/address/{walletId}/");
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CryptoCloudBaseResponse>(jsonOptions);
            
            if (result?.Status != "success")
            {
                throw new Exception($"Failed to get wallet address: {result?.Message ?? "Unknown error"}");
            }
            
            // Extract address from response
            var addressResponse = await JsonSerializer.DeserializeAsync<JsonElement>(
                await response.Content.ReadAsStreamAsync(), jsonOptions);
            
            if (addressResponse.TryGetProperty("data", out var data) && 
                data.TryGetProperty("address", out var address))
            {
                return address.GetString() ?? string.Empty;
            }
            
            throw new Exception("Failed to extract wallet address from response");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallet address from CryptoCloud");
            throw;
        }
    }

    /// <summary>
    /// Gets wallet balance
    /// </summary>
    public async Task<string> GetWalletBalanceAsync(string walletId)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/v2/wallet/balance/{walletId}/");
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CryptoCloudBaseResponse>(jsonOptions);
            
            if (result?.Status != "success")
            {
                throw new Exception($"Failed to get wallet balance: {result?.Message ?? "Unknown error"}");
            }
            
            // Extract balance from response
            var balanceResponse = await JsonSerializer.DeserializeAsync<JsonElement>(
                await response.Content.ReadAsStreamAsync(), jsonOptions);
            
            if (balanceResponse.TryGetProperty("data", out var data) && 
                data.TryGetProperty("balance", out var balance))
            {
                return balance.GetString() ?? "0";
            }
            
            throw new Exception("Failed to extract wallet balance from response");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallet balance from CryptoCloud");
            throw;
        }
    }

    /// <summary>
    /// Gets list of wallets
    /// </summary>
    public async Task<List<CryptoCloudWalletData>> GetWalletsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v2/wallet/list/");
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CryptoCloudWalletListResponse>(jsonOptions);
            
            if (result?.Status != "success" || result.Data == null)
            {
                throw new Exception($"Failed to get wallets: {result?.Message ?? "Unknown error"}");
            }
            
            return result.Data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get wallets from CryptoCloud");
            throw;
        }
    }
}
