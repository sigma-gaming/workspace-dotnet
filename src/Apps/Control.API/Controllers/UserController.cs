using Core.Contracts.Compounds;
using Core.Services.Auth;
using Core.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Control.API.Controllers;

[Authorize]
[ApiController]
public class UserController(ProfileService profileService) : ControllerBase
{
    [ProducesResponseType<UserDetails>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/user/details")]
    public async Task<Ok<UserDetails>> GetUserDetails()
    {
        var userId = HttpContext.GetUserId();
        var details = await profileService.GetUserDetailsAsync(userId);
        return TypedResults.Ok(details);
    }
}