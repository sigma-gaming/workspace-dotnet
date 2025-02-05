using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("/test")]
    public ActionResult Test()
    {
        return Ok();
    }
}