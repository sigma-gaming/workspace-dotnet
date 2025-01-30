using Core.Contracts.Compounds;
using Core.Contracts.Enums;
using Core.Services.Caching;
using Core.Services.Finance.Api;
using Microsoft.Extensions.Logging;

namespace Core.Services.Finance;

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
        [BinanceSymbol.DogeUsdt] = Currency.Doge
    };

    private async Task<CurrencyExchangeRates> GetLatestRatesAsync()
    {
        var rates = new CurrencyExchangeRates();

        try
        {
            var fiatRatesTask = coincapApi.GetRates();
            var cryptoRatesTask = binanceApi.GetTickerPrice(SymbolCurrencyMap.Keys.ToArray());

            await Task.WhenAll(fiatRatesTask, cryptoRatesTask);
            var fiatRates = fiatRatesTask.Result;
            var cryptoRates = cryptoRatesTask.Result;

            var fiatRawRatesMap = fiatRates.ToDictionary(
                rate => rate.Symbol,
                rate => double.Parse(rate.RateUsd));

            if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Rub, out var rub) &&
                fiatRawRatesMap.TryGetValue(CoincapSymbol.Usd, out var usd))
            {
                rates[Currency.Rub] = 100;
                rates[Currency.Usd] = rates[Currency.Rub] / rub;

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Eur, out var eur))
                    rates[Currency.Eur] = eur * rates[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kzt, out var kzt))
                    rates[Currency.Kzt] = kzt * rates[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Kgs, out var kgs))
                    rates[Currency.Kgs] = kgs * rates[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uzs, out var uzs))
                    rates[Currency.Uzs] = uzs * rates[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Uah, out var uah))
                    rates[Currency.Uah] = uah * rates[Currency.Usd];

                if (fiatRawRatesMap.TryGetValue(CoincapSymbol.Usdt, out var usdt))
                {
                    rates[Currency.UsdtErc20] = usdt * rates[Currency.Usd];
                    rates[Currency.UsdtTrc20] = usdt * rates[Currency.Usd];
                }

                if (rates[Currency.UsdtTrc20].HasValue)
                {
                    foreach (var ticker in cryptoRates)
                    {
                        if (SymbolCurrencyMap.TryGetValue(ticker.Symbol, out var currency))
                        {
                            rates[currency] = double.Parse(ticker.Price) * rates[Currency.UsdtTrc20]!.Value;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get currency rates");
        }

        return rates;
    }

    public async Task<CurrencyExchangeRates?> GetRatesAsync()
    {
        return await cache.CurrencyExchangeRates.GetAsync();
    }

    public async Task UpdateRatesAsync()
    {
        var rates = await GetLatestRatesAsync();
        await cache.CurrencyExchangeRates.SetAsync(rates);
    }

    public async Task<double> ConvertAsync(double amount, Currency from, Currency to)
    {
        var rates = await GetRatesAsync();
        if (rates == null)
            throw new InvalidOperationException("Currency rates not available");

        if (!rates.TryGetValue(from, out var fromRate) || !fromRate.HasValue ||
            !rates.TryGetValue(to, out var toRate) || !toRate.HasValue)
        {
            throw new InvalidOperationException("Specified currency rates not available");
        }

        return amount * fromRate.Value / toRate.Value;
    }

    public async Task<double> ConvertGemsAsync(double amount, Currency to)
    {
        var rates = await GetRatesAsync();
        if (rates == null)
            throw new InvalidOperationException("Currency rates not available");

        if (!rates.TryGetValue(to, out var toRate) || !toRate.HasValue)
            throw new InvalidOperationException("Specified currency rates not available");

        return amount / toRate.Value;
    }
}