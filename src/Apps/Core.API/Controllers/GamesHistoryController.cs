using Core.Contracts.Data;
using Core.Services.Auth;
using Core.Services.Gaming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
public class GamesHistoryController(
    GameHistoryService gameHistoryService) : ControllerBase
{
    [ProducesResponseType<GameRecord[]>(StatusCodes.Status200OK)]
    [HttpGet("/games-history/last-wins")]
    public async Task<Ok<GameRecord[]>> GetLastWins()
    {
        var lastWins = await gameHistoryService.GetLastWinHistoryAsync();
        return TypedResults.Ok(lastWins);
    }

    [ProducesResponseType<GameRecord[]>(StatusCodes.Status200OK)]
    [HttpGet("/games-history/big-wins")]
    public async Task<Ok<GameRecord[]>> GetBigWins()
    {
        var bigWins = await gameHistoryService.GetBigWinHistoryAsync();
        return TypedResults.Ok(bigWins);
    }

    [Authorize]
    [ProducesResponseType<GameRecord[]>(StatusCodes.Status200OK)]
    [HttpGet("/games-history/my-games")]
    public async Task<Ok<GameRecord[]>> GetMyGames()
    {
        var userId = HttpContext.GetUserId();
        var myGames = await gameHistoryService.GetUserGameHistoryAsync(userId);
        return TypedResults.Ok(myGames);
    }
}