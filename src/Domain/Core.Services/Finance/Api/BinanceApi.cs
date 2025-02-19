using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Services.Finance.Api;

public enum BinanceSymbol
{
    [JsonStringEnumMemberName("BTCUSDT")] BtcUsdt,
    [JsonStringEnumMemberName("ETHUSDT")] EthUsdt,
    [JsonStringEnumMemberName("LTCUSDT")] LtcUsdt,
    [JsonStringEnumMemberName("TRXUSDT")] TrxUsdt,
    [JsonStringEnumMemberName("TONUSDT")] TonUsdt,
    [JsonStringEnumMemberName("NOTUSDT")] NotUsdt,
    [JsonStringEnumMemberName("BNBUSDT")] BnbUsdt,
    [JsonStringEnumMemberName("DOGEUSDT")] DogeUsdt,
}

public sealed record BinanceTickerPrice
{
    [JsonPropertyName("symbol")] public required BinanceSymbol Symbol { get; init; }
    [JsonPropertyName("price")] public required string Price { get; init; }
}

public sealed class BinanceApi(HttpClient httpClient)
{
    private const string BASE_URL = "https://data-api.binance.vision/api/v3";

    public async Task<List<BinanceTickerPrice>> GetTickerPrice(BinanceSymbol[] symbols, CancellationToken ct = default)
    {
        var symbolsParam = string.Join(",", symbols);
        var response = await httpClient.GetAsync($"{BASE_URL}/ticker/price?symbols=[{symbolsParam}]", ct);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<List<BinanceTickerPrice>>(content);
        return result ?? throw new InvalidOperationException("Failed to deserialize Binance response");
    }
}