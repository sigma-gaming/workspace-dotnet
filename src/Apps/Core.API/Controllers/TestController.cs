using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Db.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.API.Controllers;

[ApiController]
public class TestController(
    CoreDbContext db,
    UserMapper mapper
) : ControllerBase
{
    [HttpGet("/test")]
    public async Task<ActionResult<UserPublic>> Test()
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync();

        return Ok(mapper.ToPublic(user!));
    }
}