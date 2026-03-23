using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class GeneralController : ControllerBase
{
    [HttpHead("health")]
    [HttpGet("health")]
    public IActionResult HeadHealth()
    {
        return Ok();
    }
}