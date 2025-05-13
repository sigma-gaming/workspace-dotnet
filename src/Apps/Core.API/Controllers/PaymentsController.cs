using Common.Api.Responses;
using Core.Contracts.Compounds;
using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Services.Auth;
using Core.Services.CurrencyRates;
using Core.Services.Payment;
using Core.Services.Payment.Configuration;
using Core.Services.Payment.Interfaces;
using Core.Services.Payment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    CurrencyRatesService currencyRatesService,
    WalletService walletService)
    : ControllerBase
{
    [ProducesResponseType<List<CurrencyExchangeRate>>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/payments/currency-rates")]
    public async Task<Ok<List<CurrencyExchangeRate>>> GetCurrencyRates()
    {
        var rates = await currencyRatesService.GetRatesAsync();
        return TypedResults.Ok(rates);
    }

    [HttpGet("/payments/deposit-methods")]
    [ProducesResponseType<List<DepositMethodConfig>>(StatusCodes.Status200OK)]
    public Ok<List<DepositMethodConfig>> GetDepositMethods()
    {
        var methods = paymentService.GetDepositConfigs();
        return TypedResults.Ok(methods);
    }

    [HttpGet("/payments/withdrawal-methods")]
    [ProducesResponseType<List<WithdrawalMethodConfig>>(StatusCodes.Status200OK)]
    public Ok<List<WithdrawalMethodConfig>> GetWithdrawalMethods()
    {
        var methods = paymentService.GetWithdrawalConfigs();
        return TypedResults.Ok(methods);
    }

    [Authorize]
    [HttpPost("/payments/deposit")]
    [ProducesResponseType<Deposit>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<Results<Ok<Deposit>, BadRequest<ErrorResponse>, StatusCodeHttpResult>> CreateDeposit(
        [FromBody] CreateDepositRequest request)
    {
        var userId = HttpContext.GetUserId();
        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var input = new DepositInput(
            userId,
            request.GemAmount,
            request.Method,
            request.Currency,
            request.Provider);

        var result = await paymentService.CreateDepositAsync(input);

        return result.Outcome switch
        {
            PaymentOutcome.Success => TypedResults.Ok(result.Deposit!),

            PaymentOutcome.InvalidAmount => TypedResults.BadRequest(new ErrorResponse(
                $"Неверная сумма. Минимум: {result.MinAmount}, максимум: {result.MaxAmount} {result.Currency}")),

            PaymentOutcome.UnsupportedMethod => TypedResults.BadRequest(new ErrorResponse(
                $"Неподдерживаемый метод пополнения: {result.Method} с валютой {result.Currency} через {result.Provider}")),

            PaymentOutcome.UnsupportedCurrency => TypedResults.BadRequest(new ErrorResponse(
                $"Неподдерживаемая валюта: {result.Currency}")),

            PaymentOutcome.ProviderError => TypedResults.BadRequest(new ErrorResponse(
                $"Ошибка платежного провайдера: {result.Error}")),

            _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [Authorize]
    [HttpPost("/payments/withdrawal")]
    [ProducesResponseType<Withdrawal>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<Results<Ok<Withdrawal>, BadRequest<ErrorResponse>, StatusCodeHttpResult>> CreateWithdrawal(
        [FromBody] CreateWithdrawalRequest request)
    {
        var userId = HttpContext.GetUserId();
        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var input = new WithdrawalInput(
            userId,
            request.GemAmount,
            request.Method,
            request.Currency,
            request.Provider,
            request.DestinationAddress
        );

        var result = await paymentService.CreateWithdrawalAsync(input);

        return result.Outcome switch
        {
            PaymentOutcome.Success => TypedResults.Ok(result.Withdrawal!),

            PaymentOutcome.InsufficientFunds => TypedResults.BadRequest(new ErrorResponse(
                $"Недостаточно средств. Доступно: {result.Available}")),

            PaymentOutcome.InvalidAmount => TypedResults.BadRequest(new ErrorResponse(
                $"Неверная сумма. Минимум: {result.MinAmount}, максимум: {result.MaxAmount} {result.Currency}")),

            PaymentOutcome.UnsupportedMethod => TypedResults.BadRequest(new ErrorResponse(
                $"Неподдерживаемый метод вывода: {result.Method} с валютой {result.Currency} через {result.Provider}")),

            PaymentOutcome.UnsupportedCurrency => TypedResults.BadRequest(new ErrorResponse(
                $"Неподдерживаемая валюта: {result.Currency}")),

            PaymentOutcome.ProviderError => TypedResults.BadRequest(new ErrorResponse(
                $"Ошибка платежного провайдера: {result.Error}")),

            _ => TypedResults.StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [Authorize]
    [HttpGet("/payments/wallets")]
    [ProducesResponseType<List<UserWallet>>(StatusCodes.Status200OK)]
    public async Task<Ok<List<UserWallet>>> GetUserWallets()
    {
        var userId = HttpContext.GetUserId();
        var wallets = await walletService.GetUserWalletsAsync(userId);
        return TypedResults.Ok(wallets);
    }

    [Authorize]
    [HttpPost("/payments/wallets")]
    [ProducesResponseType<UserWallet>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Results<Ok<UserWallet>, BadRequest<ErrorResponse>>> CreateWallet(
        [FromBody] CreateWalletRequest request)
    {
        var userId = HttpContext.GetUserId();

        try
        {
            var wallet = await walletService.GetOrCreateWalletAsync(userId, request.Currency, request.Provider);
            if (wallet == null) return TypedResults.BadRequest(new ErrorResponse("Не удалось создать кошелёк"));

            return TypedResults.Ok(wallet);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
    }
}

public class CreateDepositRequest
{
    public required long GemAmount { get; init; }
    public required DepositMethod Method { get; init; }
    public required Currency Currency { get; init; }
    public required PaymentProvider Provider { get; init; }
}

public class CreateWithdrawalRequest
{
    public required long GemAmount { get; init; }
    public required WithdrawalMethod Method { get; init; }
    public required Currency Currency { get; init; }
    public required PaymentProvider Provider { get; init; }
    public required string DestinationAddress { get; init; }
}

public class CreateWalletRequest
{
    public required Currency Currency { get; init; }
    public required PaymentProvider Provider { get; init; }
}