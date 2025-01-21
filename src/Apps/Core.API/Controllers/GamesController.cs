using Core.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Services.Caching;

namespace Core.API.Controllers;

public sealed record PlayDiceResponse
{
    public required Balance? Balance { get; init; }
    public required UserPublic User { get; init; }
}

[ApiController]
public class GamesController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper,
    ILogger<GamesController> logger) : ControllerBase
{
    [HttpGet("/games/play-dice")]
    public async ValueTask<ActionResult<PlayDiceResponse>> PlayDice()
    {
        var user = await db.Users.FirstAsync();

        var balance = await cache.UserBalance.GetAsync(user.Id);

        logger.LogInformation("User {UserId} is playing dice!", user.Id);
        logger.LogInformation("User balance is {Balance}", balance);

        return Ok(new PlayDiceResponse
        {
            Balance = balance,
            User = userMapper.ToPublic(user)
        });
    }
}