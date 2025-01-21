using Core.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Extensions;
using Core.Contracts.Mappers;
using Core.Services.Caching;

namespace Core.API.Controllers;

public sealed record PlayDiceResponse
{
    public required BalanceData? Balance { get; init; }
    public required UserData User { get; init; }
}

[ApiController]
public class GamesController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper,
    ILogger<GamesController> logger) : ControllerBase
{
    [HttpGet("/games/play-dice")]
    [EndpointName("Dice"), EndpointDescription("Play dice")]
    public async ValueTask<ActionResult<PlayDiceResponse>> PlayDice()
    {
        var user = await db.Users.FirstAsync();

        var balance = await cache.UserBalance.GetAsync(new Guid("01947e4e-cf6a-7e76-8e38-261aef5ecf73"));

        logger.LogInformation("User {UserId} is playing dice!", user.Id);
        logger.LogInformation("User balance is {Balance}", balance);

        return Ok(new PlayDiceResponse
        {
            Balance = balance,
            User = userMapper.ToData(user)
        });
    }
}