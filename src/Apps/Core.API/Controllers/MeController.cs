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
    ProfileService profileService,
    BalanceService balanceService,
    BalanceMapper balanceMapper) : ControllerBase
{
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK)]
    [HttpGet("/me/user-details")]
    public async Task<Ok<UserDetails>> GetUserDetails()
    {
        var userId = HttpContext.GetUserId();
        var details = await profileService.GetUserDetailsAsync(userId);
        return TypedResults.Ok(details);
    }

    [ProducesResponseType<BalancePublic>(StatusCodes.Status200OK)]
    [HttpGet("/me/balance")]
    public async Task<Ok<BalancePublic>> GetDetailedBalance()
    {
        var userId = HttpContext.GetUserId();
        var balance = await balanceService.GetBalanceAsync(userId);
        return TypedResults.Ok(balanceMapper.ToPublic(balance));
    }
}