
using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/rotation-type")]
public class RotationTypeController(MedicalContext context) : ControllerBase
{
    private readonly MedicalContext context = context;

    [HttpGet("{id}")]
    public async Task<ActionResult<RotationTypeResponse>> GetById([FromRoute] Guid id)
    {
        RotationType? foundRotationType = context.RotationTypes.Find(id);

        if (foundRotationType == null)
        {
            return NotFound();
        }

        RotationTypeResponse response = RotationTypeConverter.CreateRotationTypeResponse(foundRotationType);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<RotationTypesListResponse>> GetAll([FromQuery] List<int> pgyYear)
    {
        // Validations
        foreach (int year in pgyYear)
        {
            if (year < 0 || year > 4)
            {
                return BadRequest("Pgy Year cannot be less than 0 or greater than 4");
            }
        }

        List<RotationType> rotationTypes;

        if (pgyYear.Count == 0)
        {
            rotationTypes = [.. context.RotationTypes];
        }
        else
        {
            PgyYearFlags searchFlags = 0;
            foreach (int year in pgyYear)
            {
                searchFlags |= (PgyYearFlags)(1 << (year - 1));
            }

            rotationTypes = [.. context.RotationTypes.Where(rt => (rt.PgyYearFlags & searchFlags) != 0)];
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