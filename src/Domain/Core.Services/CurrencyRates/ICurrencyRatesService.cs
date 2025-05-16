using Core.Contracts.Compounds;
using Core.Contracts.Enums;

namespace Core.Services.CurrencyRates;

/// <summary>
///     Interface for the currency rates service
/// </summary>
public interface ICurrencyRatesService
{
    /// <summary>
    ///     Gets the current exchange rates
    /// </summary>
    /// <returns>List of currency exchange rates</returns>
    Task<List<CurrencyExchangeRate>> GetRatesAsync();

    /// <summary>
    ///     Updates the exchange rates
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    Task UpdateRatesAsync(CancellationToken ct = default);

    /// <summary>
    ///     Converts an amount from one currency to another
    /// </summary>
    /// <param name="amount">Amount to convert</param>
    /// <param name="from">Source currency</param>
    /// <param name="to">Target currency</param>
    /// <returns>Converted amount</returns>
    Task<double> ConvertAsync(double amount, Currency from, Currency to);

    /// <summary>
    ///     Converts an amount in gems to a specific currency
    /// </summary>
    /// <param name="amount">Amount in gems</param>
    /// <param name="to">Target currency</param>
    /// <returns>Converted amount</returns>
    Task<double> ConvertGemsAsync(double amount, Currency to);
}
