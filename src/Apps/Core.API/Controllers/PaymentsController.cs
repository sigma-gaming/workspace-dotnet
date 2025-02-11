using Core.Contracts.Compounds;
using Core.Services.Finance;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
public class PaymentsController(
    CurrencyRatesService currencyRatesService) : ControllerBase
{
    [ProducesResponseType<CurrencyExchangeRates>(StatusCodes.Status200OK)]
    [HttpGet("/payments/currency-rates")]
    public async Task<Ok<CurrencyExchangeRates>> GetCurrencyRates()
    {
        var rates = await currencyRatesService.GetRatesAsync();
        return TypedResults.Ok(rates ?? new CurrencyExchangeRates());
    }
}