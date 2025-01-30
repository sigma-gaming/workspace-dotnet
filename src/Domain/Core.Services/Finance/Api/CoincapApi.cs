using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Services.Finance.Api;

public enum CoincapSymbol
{
    [JsonStringEnumMemberName("RUB")] Rub,
    [JsonStringEnumMemberName("KZT")] Kzt,
    [JsonStringEnumMemberName("KGS")] Kgs,
    [JsonStringEnumMemberName("UZS")] Uzs,
    [JsonStringEnumMemberName("UAH")] Uah,
    [JsonStringEnumMemberName("USD")] Usd,
    [JsonStringEnumMemberName("EUR")] Eur,
    [JsonStringEnumMemberName("USDT")] Usdt
}

public sealed record CoincapRate
{
    [JsonPropertyName("symbol")] public required CoincapSymbol Symbol { get; init; }

    [JsonPropertyName("rateUsd")] public required string RateUsd { get; init; }
}

public sealed record CoincapRatesResponse
{
    [JsonPropertyName("data")] public required List<CoincapRate> Data { get; init; }
}

public sealed class CoincapApi(HttpClient httpClient)
{
    private const string BaseUrl = "https://api.coincap.io/v2";

    public async Task<List<CoincapRate>> GetRates()
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/rates");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CoincapRatesResponse>(content);
        return result?.Data ?? throw new InvalidOperationException("Failed to deserialize Coincap response");
    }
}