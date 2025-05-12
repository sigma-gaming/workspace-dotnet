using System.Globalization;
using Core.Contracts.Compounds;
using Core.Contracts.Enums;
using Core.Services.Caching;
using Core.Services.CurrencyRates.Api;
using Microsoft.Extensions.Logging;

namespace Core.Services.CurrencyRates;

public class CurrencyRatesService(
    ILogger<CurrencyRatesService> logger,
    CoreCacheRegistry cache,
    BinanceApi binanceApi,
    CoincapApi coincapApi)
{
    private static readonly Dictionary<BinanceSymbol, Currency> SymbolCurrencyMap = new()
    {
        [BinanceSymbol.BtcUsdt] = Currency.Btc,
        [BinanceSymbol.EthUsdt] = Currency.Eth,
        [BinanceSymbol.LtcUsdt] = Currency.Ltc,
        [BinanceSymbol.TrxUsdt] = Currency.Trx,
        [BinanceSymbol.TonUsdt] = Currency.Ton,
        [BinanceSymbol.NotUsdt] = Currency.Not,
        [BinanceSymbol.BnbUsdt] = Currency.Bnb,
        [BinanceSymbol.DogeUsdt] = Currency.Doge,
    };

    private async Task<CurrencyExchangeRates> GetLatestRatesAsync(CancellationToken ct = default)
    {
        var rates = new CurrencyExchangeRates();
        var map = rates.Map;

        try
        {
            var fiatRatesTask = coincapApi.GetRates(ct);
            var cryptoRatesTask = binanceApi.GetTickerPrice(SymbolCurrencyMap.Keys.ToArray(), ct);

            await Task.WhenAll(fiatRatesTask, cryptoRatesTask);
            var fiatRates = fiatRatesTask.Result;
            var cryptoRates = cryptoRatesTask.Result;

            var fiatRawRatesMap = fiatRates
                .GroupBy(rate => rate.Symbol)
                .ToDictionary(
                    group => group.Key,
                    group => (double)decimal.Parse(group.First().RateUsd, CultureInfo.InvariantCulture));

            if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Rub, out var rub) &&
                fiatRawRatesMap.TryGetValue(CoincapSymbol.Usd, out var usd))
            {
                map[Currency.Rub] = 100;
                map[Currency.Usd] = map[Currency.Rub] / rub;

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Eur, out var eur))
                    map[Currency.Eur] = eur * map[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kzt, out var kzt))
                    map[Currency.Kzt] = kzt * map[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kgs, out var kgs))
                    map[Currency.Kgs] = kgs * map[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uzs, out var uzs))
                    map[Currency.Uzs] = uzs * map[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uah, out var uah))
                    map[Currency.Uah] = uah * map[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Usdt, out var usdt))
                {
                    map[Currency.UsdtErc20] = usdt * map[Currency.Usd];
                    map[Currency.UsdtTrc20] = usdt * map[Currency.Usd];
                }

                if (map[Currency.UsdtTrc20].HasValue)
                    foreach (var ticker in cryptoRates)
                        if (SymbolCurrencyMap.TryGetValue(ticker.Symbol, out var currency))
                            map[currency] = double.Parse(ticker.Price, CultureInfo.InvariantCulture) *
                                            map[Currency.UsdtTrc20]!.Value;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get currency rates");
            throw;
        }

        return rates;
    }

    public async Task<CurrencyExchangeRates?> GetRatesAsync()
    {
        return await cache.CurrencyExchangeRates.GetAsync();
    }

    public async Task UpdateRatesAsync(CancellationToken ct = default)
    {
        var rates = await GetLatestRatesAsync(ct);
        await cache.CurrencyExchangeRates.SetAsync(rates);
    }

    public async Task<double> ConvertAsync(double amount, Currency from, Currency to)
    {
        var rates = await GetRatesAsync();
        if (rates is null)
            throw new InvalidOperationException("Currency rates not available");

        if (!rates.Map.TryGetValue(from, out var fromRate) || !fromRate.HasValue ||
            !rates.Map.TryGetValue(to, out var toRate) || !toRate.HasValue)
            throw new InvalidOperationException("Specified currency rates not available");

        return amount * fromRate.Value / toRate.Value;
    }

    public async Task<double> ConvertGemsAsync(double amount, Currency to)
    {
        var rates = await GetRatesAsync();
        if (rates is null)
            throw new InvalidOperationException("Currency rates not available");

        if (!rates.Map.TryGetValue(to, out var toRate) || !toRate.HasValue)
            throw new InvalidOperationException("Specified currency rates not available");

        return amount / toRate.Value;
    }
}