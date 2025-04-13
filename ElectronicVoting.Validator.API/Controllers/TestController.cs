using Microsoft.AspNetCore.Mvc;

namespace ElectronicVoting.Validator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpGet("Test")]
    public IActionResult Test()
    {
        return Ok(1);
    }
}