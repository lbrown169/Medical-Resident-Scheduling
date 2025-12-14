using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class GeneralController : ControllerBase
{
    [HttpHead("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}