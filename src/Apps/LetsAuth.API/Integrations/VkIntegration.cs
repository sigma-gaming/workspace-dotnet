using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Services.Vk;
using Microsoft.Extensions.Options;

namespace LetsAuth.API.Integrations;

public class VkIntegration(HttpClient httpClient, ILogger<VkIntegration> logger, IOptions<VkOptions> options)
{
    private static readonly JsonSerializerOptions VkJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async Task<VkTokenExchangeResponse> ExchangeSilentAuthToken(string token, string uuid)
    {
        try
        {
            var requestUrl =
                $"https://api.vk.com/method/auth.exchangeSilentAuthToken?v=5.199&token={token}&access_token={options.Value.ServiceToken}&uuid={uuid}";

            var response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<VkResponse<VkTokenExchangeResponse>>(content, VkJsonOptions);

            if (responseObj?.Response != null) return responseObj.Response;
            logger.LogError("Failed to exchange VK silent auth token. Response: {Response}", content);
            throw new Exception("Failed to exchange VK auth token");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error when exchanging VK silent auth token");
            throw new Exception("Failed to connect to VK API");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON parsing error when exchanging VK silent auth token");
            throw new Exception("Invalid response from VK API");
        }
    }

    public async Task<VkProfileResponse> GetProfile(string accessToken)
    {
        try
        {
            var requestUrl = $"https://api.vk.com/method/account.getProfileInfo?v=5.199&access_token={accessToken}";

            var response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<VkResponse<VkProfileResponse>>(content, VkJsonOptions);

            if (responseObj?.Response != null) return responseObj.Response;
            logger.LogError("Failed to get VK profile. Response: {Response}", content);
            throw new Exception("Failed to get VK profile");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error when getting VK profile");
            throw new Exception("Failed to connect to VK API");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON parsing error when getting VK profile");
            throw new Exception("Invalid response from VK API");
        }
    }

    private class VkResponse<T>
    {
        public T? Response { get; set; }
        public VkError? Error { get; set; }
    }

    private class VkError
    {
        public int ErrorCode { get; set; }
        public string? ErrorMsg { get; set; }
    }
}

public sealed record VkAuthData
{
    public required string Type { get; init; }
    public required string Uuid { get; init; }
    public required int Auth { get; init; }
    public required string Token { get; init; }
    public required int Ttl { get; init; }
    public required string Hash { get; init; }
    public required VkUser User { get; init; }
}

public sealed record VkUser
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Avatar { get; init; }
}

public sealed record VkTokenExchangeResponse
{
    public required string AccessToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required int UserId { get; init; }
}

public sealed record VkProfileResponse
{
    public required int Id { get; init; }
    [JsonPropertyName("photo_200")] public string? Photo200 { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? ScreenName { get; init; }
}