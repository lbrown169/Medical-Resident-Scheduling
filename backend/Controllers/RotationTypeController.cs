
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/rotation-types")]
public class RotationTypeController(MedicalContext context) : ControllerBase
{
    private readonly MedicalContext context = context;

    // GET: api/rotation-types/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RotationTypeResponse>> GetById([FromRoute] Guid id)
    {
        RotationType? foundRotationType = await context.RotationTypes.FindAsync(id);

        if (foundRotationType == null)
        {
            return NotFound();
        }

        RotationTypeResponse response = RotationTypeConverter.CreateRotationTypeResponse(foundRotationType);
        return Ok(response);
    }

    // GET: api/rotation-types
    [HttpGet]
    public async Task<ActionResult<RotationTypesListResponse>> GetAll([FromQuery] List<int> pgyYear)
    {
        // Validations
        foreach (int year in pgyYear)
        {
            if (year < 1 || year > 4)
            {
                return BadRequest("Pgy Year cannot be less than 1 or greater than 4");
            }
        }

        List<RotationType> rotationTypes;

        if (pgyYear.Count == 0)
        {
            rotationTypes = await context.RotationTypes.ToListAsync();
        }
        else
        {
            PgyYearFlags searchFlags = 0;
            foreach (int year in pgyYear)
            {
                searchFlags |= (PgyYearFlags)(1 << (year - 1));
            }

            rotationTypes = await context.RotationTypes.Where(rt => (rt.PgyYearFlags & searchFlags) != 0).ToListAsync();
        }

        List<RotationTypeResponse> rotationTypeResponses = [.. rotationTypes.Select(RotationTypeConverter.CreateRotationTypeResponse)];
        RotationTypesListResponse finalResponse = new()
        {
            Count = rotationTypeResponses.Count,
            RotationTypes = rotationTypeResponses
        };
        return Ok(finalResponse);
    }
}