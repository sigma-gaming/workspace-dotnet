using System.ComponentModel.DataAnnotations;
using Core.Contracts.Enums;

namespace Core.Contracts.Compounds;

public sealed record CurrencyExchangeRates
{
    [Required] public Dictionary<Currency, double?> Map { get; init; } = new();
}