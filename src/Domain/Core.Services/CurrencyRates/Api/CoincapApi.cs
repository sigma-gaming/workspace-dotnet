using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Core.Services.CurrencyRates.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CoincapSymbol
{
    [JsonStringEnumMemberName("RUB")] Rub,
    [JsonStringEnumMemberName("KZT")] Kzt,
    [JsonStringEnumMemberName("KGS")] Kgs,
    [JsonStringEnumMemberName("UZS")] Uzs,
    [JsonStringEnumMemberName("UAH")] Uah,
    [JsonStringEnumMemberName("USD")] Usd,
    [JsonStringEnumMemberName("EUR")] Eur,
    [JsonStringEnumMemberName("USDT")] Usdt,
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

public sealed class CoincapApi(HttpClient httpClient, IOptions<CoincapOptions> options)
{
    private const string BASE_URL = "https://rest.coincap.io/v3";

    private static readonly string[] CurrencyIds =
    [
        "russian-ruble",
        "kazakhstani-tenge",
        "kyrgystani-som",
        "uzbekistan-som",
        "ukrainian-hryvnia",
        "united-states-dollar",
        "euro",
        "tether",
    ];

    public async Task<List<CoincapRate>> GetRates(CancellationToken ct = default)
    {
        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Coincap API key is not configured");

        var response =
            await httpClient.GetAsync($"{BASE_URL}/rates?apiKey={apiKey}&ids={string.Join(",", CurrencyIds)}", ct);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<CoincapRatesResponse>(content);
        return result?.Data ?? throw new InvalidOperationException("Failed to deserialize Coincap response");
    }
}