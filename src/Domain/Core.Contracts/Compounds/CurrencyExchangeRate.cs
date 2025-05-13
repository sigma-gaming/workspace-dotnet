using System.ComponentModel.DataAnnotations;
using Core.Contracts.Enums;

namespace Core.Contracts.Compounds;

public sealed record CurrencyExchangeRate
{
    [Required] public Currency Currency { get; init; }
    [Required] public double Gems { get; init; }
}