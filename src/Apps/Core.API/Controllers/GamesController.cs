using Core.Db.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Data;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Services.Auth;
using Core.Services.Caching;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace Core.API.Controllers;

public sealed record PlayDiceResponse
{
    public required Balance? Balance { get; init; }
    public required UserPublic? User { get; init; }
}

[ApiController]
public class GamesController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper) : ControllerBase
{
    [Authorize]
    [HttpGet("/games/play-dice")]
    public async ValueTask<ActionResult<PlayDiceResponse>> PlayDice()
    {
        var userId = HttpContext.GetUserId();
        Log.Information("UserId: {UserId}", userId);
        
        var user = await db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null)
        {
            Log.Warning("User {UserId} is not found", userId);
            return Unauthorized();
        }
        
        var balance = await cache.UserBalance.GetAsync(userId);

        Log.Information("User {UserId} is playing dice!", userId);
        Log.Information("User balance is {Balance}", balance);

        return Ok(new PlayDiceResponse
        {
            Balance = balance,
            User = userMapper.ToPublic(user)
        });
    }
}