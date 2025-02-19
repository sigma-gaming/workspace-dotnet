using System.ComponentModel.DataAnnotations;
using Common.Validation.Attributes;
using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Services.Auth;
using Core.Services.Gaming.Runners;
using Core.Services.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

public record BasePlayPayload
{
    [Required(ErrorMessage = "Необходимо выбрать ставку")]
    [Range(1_00, 5000_00, ErrorMessage = "Допустимы ставки от 1 до 5000 гемов")]
    public required long Bet { get; init; }
}

public sealed record PlayDicePayload : BasePlayPayload
{
    [Required(ErrorMessage = "Необходимо выбрать стороны")]
    [EachInRange(1, 6, ErrorMessage = "Сторона должна быть от 1 до 6")]
    [Length(1, 5, ErrorMessage = "Можно выбрать до 5 сторон")]
    public required int[] Sides { get; init; }
}

public sealed record PlayPincodePayload : BasePlayPayload
{
    [Required(ErrorMessage = "Необходимо выбрать режим")]
    public required PincodeMode Mode { get; init; }
}

public sealed record PlayResponse
{
    public required GameRecord Record { get; init; }
    public required UserUpdate<BalancePublic> BalanceUpdate { get; init; }
}

[ApiController]
public class GamesController(
    CorePubSubRegistry pubSub,
    BalanceMapper balanceMapper,
    GameRecordMapper gameRecordMapper,
    DiceGame diceGame,
    PincodeGame pincodeGame) : ControllerBase
{
    [ProducesResponseType<string>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/games/ping")]
    public Ok<string> Ping()
    {
        return TypedResults.Ok("pong");
    }

    [Authorize]
    [ProducesResponseType<PlayResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/games/play-dice")]
    public async Task<Results<Ok<PlayResponse>, ValidationProblem>> PlayDice([FromBody] PlayDicePayload payload)
    {
        var userId = HttpContext.GetUserId();

        try
        {
            var playResult = await diceGame.PlayAsync(userId, new DiceGamePayload
            {
                Bet = payload.Bet,
                Sides = payload.Sides,
            });

            var balanceUpdate = new UserUpdate<BalancePublic>
            {
                UserId = userId,
                Mode = UpdateMode.Optimized,
                Data = balanceMapper.ToPublic(playResult.UpdatedBalance),
            };

            await pubSub.BalanceUpdated.PublishAsync(balanceUpdate);

            return TypedResults.Ok(new PlayResponse
            {
                Record = gameRecordMapper.ToData(playResult.GameRecord),
                BalanceUpdate = balanceUpdate,
            });
        }
        catch (InsufficientBalanceException)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(PlayDicePayload.Bet)] = ["Недостаточно гемов"],
                });
        }
    }

    [Authorize]
    [ProducesResponseType<PlayResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/games/play-pincode")]
    public async Task<Results<Ok<PlayResponse>, ValidationProblem>> PlayPincode([FromBody] PlayPincodePayload payload)
    {
        var userId = HttpContext.GetUserId();

        try
        {
            var playResult = await pincodeGame.PlayAsync(userId, new PincodeGamePayload
            {
                Bet = payload.Bet,
                Mode = payload.Mode,
            });

            var balanceUpdate = new UserUpdate<BalancePublic>
            {
                UserId = userId,
                Mode = UpdateMode.Optimized,
                Data = balanceMapper.ToPublic(playResult.UpdatedBalance),
            };

            await pubSub.BalanceUpdated.PublishAsync(balanceUpdate);

            return TypedResults.Ok(new PlayResponse
            {
                Record = gameRecordMapper.ToData(playResult.GameRecord),
                BalanceUpdate = balanceUpdate,
            });
        }
        catch (InsufficientBalanceException)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(PlayPincodePayload.Bet)] = ["Недостаточно гемов"],
                });
        }
    }
}