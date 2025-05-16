using Core.Contracts.Compounds;
using Core.Contracts.Enums;
using Core.Services.CurrencyRates;

namespace Core.Services.Tests.Payment;

/// <summary>
///     Mock CurrencyRatesService implementation for tests
/// </summary>
public class TestCurrencyRatesService : ICurrencyRatesService
{
    /// <summary>
    ///     Fixed exchange rates for testing
    /// </summary>
    private static readonly List<CurrencyExchangeRate> TestRates = new()
    {
        new CurrencyExchangeRate { Currency = Currency.Rub, Gems = 100 },
        new CurrencyExchangeRate { Currency = Currency.Usd, Gems = 1000 },
        new CurrencyExchangeRate { Currency = Currency.Eur, Gems = 1100 },
        new CurrencyExchangeRate { Currency = Currency.UsdtTrc20, Gems = 1000 },
        new CurrencyExchangeRate { Currency = Currency.UsdtErc20, Gems = 1000 },
        new CurrencyExchangeRate { Currency = Currency.Btc, Gems = 20000000 },
        new CurrencyExchangeRate { Currency = Currency.Eth, Gems = 2000000 },
        new CurrencyExchangeRate { Currency = Currency.Ton, Gems = 5000 }
    };

    /// <summary>
    ///     Returns the fixed test rates
    /// </summary>
    public Task<List<CurrencyExchangeRate>> GetRatesAsync()
    {
        return Task.FromResult(TestRates);
    }

    /// <summary>
    ///     Update is not needed for tests - just returns
    /// </summary>
    public Task UpdateRatesAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Convert from one currency to another using fixed test rates
    /// </summary>
    public Task<double> ConvertAsync(double amount, Currency from, Currency to)
    {
        var fromRate = TestRates.FirstOrDefault(r => r.Currency == from);
        if (fromRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        var toRate = TestRates.FirstOrDefault(r => r.Currency == to);
        if (toRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        return Task.FromResult(amount * fromRate.Gems / toRate.Gems);
    }

    /// <summary>
    ///     Convert gems to currency using fixed test rates
    /// </summary>
    public Task<double> ConvertGemsAsync(double amount, Currency to)
    {
        var toRate = TestRates.FirstOrDefault(r => r.Currency == to);
        if (toRate is null)
            throw new InvalidOperationException("Specified currency rates not available");

        return Task.FromResult(amount / toRate.Gems);
    }
}
