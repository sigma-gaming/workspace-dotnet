using Core.Contracts.Compounds;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Services.Auth;
using Core.Services.Finance;
using Core.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[Authorize]
[ApiController]
public class MeController(
    UserService userService,
    ProfileService profileService,
    BalanceService balanceService,
    UserMapper userMapper,
    BalanceMapper balanceMapper) : ControllerBase
{
    [ProducesResponseType<UserPublic>(StatusCodes.Status200OK)]
    [HttpGet("/me/get-user")]
    public async Task<Ok<UserPublic>> GetUser()
    {
        var userId = HttpContext.GetUserId();
        var user = await userService.GetUserAsync(userId);
        return TypedResults.Ok(userMapper.ToPublic(user));
    }

    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [HttpGet("/me/get-user-details")]
    public async Task<Ok<UserDetails>> GetUserDetails()
    {
        var userId = HttpContext.GetUserId();
        var details = await profileService.GetUserDetailsAsync(userId);
        return TypedResults.Ok(details);
    }

    [ProducesResponseType<BalancePublic>(StatusCodes.Status200OK)]
    [HttpGet("/me/get-balance")]
    public async Task<Ok<BalancePublic>> GetDetailedBalance()
    {
        var userId = HttpContext.GetUserId();
        var balance = await balanceService.GetBalanceAsync(userId);
        return TypedResults.Ok(balanceMapper.ToPublic(balance));
    }
}