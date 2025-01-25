using Core.Db.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Services.Auth;
using Core.Services.Caching;
using Microsoft.AspNetCore.Authorization;

namespace Core.API.Controllers;

public sealed record PlayDiceResponse
{
    public required Balance? Balance { get; init; }
    public required UserPublic? User { get; init; }
}

[ApiController]
public class GamesController(
    ILogger<GamesController> logger,
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper) : ControllerBase
{
    [Authorize]
    [HttpGet("/games/play-dice")]
    public async ValueTask<ActionResult<PlayDiceResponse>> PlayDice()
    {
        var userId = HttpContext.GetUserId();
        logger.LogInformation("UserId: {UserId}", userId);

        var user = await db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null)
        {
            logger.LogWarning("User {UserId} is not found", userId);
            return Unauthorized();
        }

        var balance = await cache.UserBalance.GetAsync(userId);

        logger.LogInformation("User {UserId} is playing dice!", userId);
        logger.LogInformation("User balance is {Balance}", balance);

        return Ok(new PlayDiceResponse
        {
            Balance = balance,
            User = userMapper.ToPublic(user)
        });
    }
}