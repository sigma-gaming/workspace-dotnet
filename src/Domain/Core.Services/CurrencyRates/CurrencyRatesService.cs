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

    private async Task<List<CurrencyExchangeRate>> GetLatestRatesAsync(CancellationToken ct = default)
    {
        List<CurrencyExchangeRate> rates = [];

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
                    group => (double)decimal.Parse(group.First().RateUsd));

            if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Rub, out var rub) &&
                fiatRawRatesMap.TryGetValue(CoincapSymbol.Usd, out var usd))
            {
                rates.Add(new CurrencyExchangeRate { Currency = Currency.Rub, Gems = 100 });

                var usdRate = usd / rub * 100;
                logger.LogInformation("USD rate: {UsdRate}", usdRate);
                rates.Add(new CurrencyExchangeRate { Currency = Currency.Usd, Gems = usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Eur, out var eur))
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.Eur, Gems = eur * usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kzt, out var kzt))
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.Kzt, Gems = kzt * usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kgs, out var kgs))
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.Kgs, Gems = kgs * usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uzs, out var uzs))
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.Uzs, Gems = uzs * usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uah, out var uah))
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.Uah, Gems = uah * usdRate });

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Usdt, out var usdt))
                {
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.UsdtErc20, Gems = usdt * usdRate });
                    rates.Add(new CurrencyExchangeRate { Currency = Currency.UsdtTrc20, Gems = usdt * usdRate });
                }

                if (rates.Any(r => r.Currency == Currency.UsdtTrc20))
                    foreach (var ticker in cryptoRates)
                        if (SymbolCurrencyMap.TryGetValue(ticker.Symbol, out var currency))
                            rates.Add(new CurrencyExchangeRate
                            {
                                Currency = currency,
                                Gems = double.Parse(ticker.Price) * usdRate,
                            });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get currency rates");
            throw;
        }

        return rates;
    }

    public async Task<List<CurrencyExchangeRate>> GetRatesAsync()
    {
        return await cache.CurrencyExchangeRates.GetAsync() ?? [];
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

        var fromRate = rates.FirstOrDefault(r => r.Currency == from);
        if (fromRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        var toRate = rates.FirstOrDefault(r => r.Currency == to);
        if (toRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        return amount * fromRate.Gems / toRate.Gems;
    }

    public async Task<double> ConvertGemsAsync(double amount, Currency to)
    {
        var rates = await GetRatesAsync();
        if (rates is null)
            throw new InvalidOperationException("Currency rates not available");

        var toRate = rates.FirstOrDefault(r => r.Currency == to);
        if (toRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        return amount / toRate.Gems;
    }
}