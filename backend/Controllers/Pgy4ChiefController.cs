using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/pgy4-chief")]
public class Pgy4ChiefController(MedicalContext context, ResidentConverter residentConverter)
    : ControllerBase
{
    private readonly MedicalContext context = context;
    private readonly ResidentConverter residentConverter = residentConverter;

    [HttpPatch("{residentId}")]
    public async Task<ActionResult<ResidentResponse>> SetResidentChiefType(
        [FromRoute] string residentId,
        [FromBody] Pgy4UpdateResidentChiefTypeRequest updateChiefTypeRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Resident? resident = await context.Residents.FirstOrDefaultAsync(
            (resident) => resident.ResidentId == residentId
        );

        // Check resident existence
        if (resident == null)
        {
            return NotFound();
        }

        // Check resident PGY year
        if (resident.GraduateYr != 3)
        {
            ModelState.AddModelError("Invalid Graduate Year", "Resident's graduate year must be 3");
            return BadRequest(ModelState);
        }

        resident.ChiefType = updateChiefTypeRequest.ChiefType;
        await context.SaveChangesAsync();

        ResidentResponse residentResponse = residentConverter.CreateResidentResponseFromResident(
            resident
        );
        return Ok(residentResponse);
    }
}