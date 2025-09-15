using Microsoft.AspNetCore.Mvc;

namespace LibraryRestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [Route("/health")]
    public IActionResult Health()
    {
        Console.WriteLine(DateTime.Now);
        return Ok(new { status = "ok" });
    }
}