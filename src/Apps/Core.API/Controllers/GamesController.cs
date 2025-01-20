using Core.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Extensions;
using Core.Services.Caching;

namespace Core.API.Controllers;

[ApiController]
public class GamesController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ILogger<GamesController> logger) : ControllerBase
{
    [HttpGet("/games/play-dice")]
    [EndpointName("Dice"), EndpointDescription("Play dice")]
    public async ValueTask<ActionResult<UserData>> PlayDice()
    {
        var user = await db.Users.FirstAsync();

        var balance = await cache.UserBalance.GetAsync(user.Id);

        logger.LogInformation("User {UserId} is playing dice!", user.Id);
        logger.LogInformation("User balance is {Balance}", balance);

        return Ok(user.ToData());
    }
}