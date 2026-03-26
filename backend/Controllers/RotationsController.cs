using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RotationsController(
    MedicalContext context,
    RotationConverter rotationConverter,
    RotationTypeConverter rotationTypeConverter)
    : ControllerBase
{
    // GET: api/rotations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> GetRotations(
        [FromQuery] int? pgyYear,
        [FromQuery] int? academicYear
    )
    {
        IQueryable<Rotation> query = context.Rotations
            .Include(rotation => rotation.RotationType)
            .AsQueryable();

        if (pgyYear != null)
        {
            query = query.Where(v => v.PgyYear == pgyYear);
        }

        if (academicYear != null)
        {
            query = query.Where(v => v.AcademicYear == academicYear);
        }

        List<RotationResponse> responses =
            (await query.ToListAsync())
            .Select(rotationConverter.CreateRotationResponseFromModel)
            .ToList();

        return responses;
    }

    // PUT: api/rotations/{id}/assign
    [HttpPut("{id}/assign")]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> AssignRotation(
        [FromRoute] Guid id,
        [FromBody] RotationAssignRequest assignRequest)
    {
        List<Rotation> allRotations = await context.Rotations
            .Include(rotation => rotation.RotationType)
            .ToListAsync();
        List<Rotation> existingRotations = allRotations.Where(r => r.RotationId == id).ToList();
        if (existingRotations.Count == 0)
        {
            return NotFound(GenericResponse.Failure("Rotation not found"));
        }

        Resident? resident = await context.Residents.FindAsync(assignRequest.ResidentId);
        if (resident == null)
        {
            return NotFound(GenericResponse.Failure("Resident not found"));
        }

        if (resident.GraduateYr == null)
        {
            return BadRequest(GenericResponse.Failure("Resident does not have PGY set"));
        }

        int newRotationYear = existingRotations.First().AcademicYear;
        foreach (Rotation rotation in allRotations.Where(r =>
                     r.AcademicYear == newRotationYear && r.ResidentId == assignRequest.ResidentId))
        {
            rotation.ResidentId = null;
        }

        // Update ResidentId field
        foreach (Rotation rotation in existingRotations)
        {
            int yearOffset = rotation.AcademicYear - DateTime.Now.AcademicYear;
            int residentGraduateYr = resident.GraduateYr.Value + yearOffset;
            if (rotation.PgyYear != residentGraduateYr)
            {
                return BadRequest(
                    GenericResponse.Failure(
                        $"Resident will be PGY{residentGraduateYr}, which is not allowed in rotation"
                    )
                );
            }

            rotation.ResidentId = assignRequest.ResidentId;
        }

        try
        {
            await context.SaveChangesAsync();
            return Ok(existingRotations.Select(rotationConverter.CreateRotationResponseFromModel));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the rotation: {ex.Message}");
        }
    }

    // PUT: api/rotations/{id}/unassign
    [HttpPut("{id}/unassign")]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> UnassignRotation([FromRoute] Guid id)
    {
        List<Rotation> existingRotations = await context.Rotations
            .Include(rotation => rotation.RotationType)
            .Where(r => r.RotationId == id).ToListAsync();

        if (existingRotations.Count == 0)
        {
            return NotFound(GenericResponse.Failure("Rotation not found"));
        }

        foreach (Rotation rotation in existingRotations)
        {
            rotation.ResidentId = null;
        }

        try
        {
            await context.SaveChangesAsync();
            return Ok(existingRotations.Select(rotationConverter.CreateRotationResponseFromModel));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the rotation: {ex.Message}");
        }
    }

    // PATCH: api/rotations/{id}/{calendarMonth}
    [HttpPatch("{id}/{academicMonth}")]
    public async Task<ActionResult<RotationResponse>> UpdateRotationMonth(
        [FromRoute] Guid id,
        [FromRoute] int academicMonth,
        [FromBody] RotationMonthUpdateRequest updateRequest
    )
    {
        if (academicMonth is < 0 or > 11)
        {
            return BadRequest(GenericResponse.Failure("Academic month must be between 0 and 11"));
        }

        Rotation? existingRotation = await context.Rotations
            .Include(rotation => rotation.RotationType)
            .FirstOrDefaultAsync(r => r.RotationId == id && r.AcademicMonthIndex == MonthOfYearExtensions.FromAcademicIndex(academicMonth, false));
        if (existingRotation == null)
        {
            return NotFound(GenericResponse.Failure("Rotation not found"));
        }

        RotationType? rotationType = await context.RotationTypes.FindAsync(updateRequest.RotationTypeId);
        if (rotationType == null)
        {
            return NotFound(GenericResponse.Failure("Rotation type not found"));
        }

        if (!rotationType.PgyYearFlags.ContainsYear(existingRotation.PgyYear))
        {
            return BadRequest(
                GenericResponse.Failure("Rotation type not valid for rotation (PGY Year)"));
        }

        existingRotation.RotationTypeId = updateRequest.RotationTypeId;
        await context.SaveChangesAsync();
        return Ok(rotationConverter.CreateRotationResponseFromModel(existingRotation));
    }

    // GET: api/rotations/copyable
    [HttpGet("copyable")]
    public async Task<ActionResult<IEnumerable<int>>> GetCopyableAcademicYears()
    {
        List<int> copyable = await context.Rotations
            .Where(r => r.PgyYear == 1 || r.PgyYear == 2)
            .Select(r => r.AcademicYear)
            .Distinct()
            .ToListAsync();

        return Ok(copyable);
    }

    // POST: api/rotations/copy
    [HttpPost("copy")]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> CopyRotations(
        [FromBody] CopyRotationRequest copyRequest
    )
    {
        List<Rotation> rotations = await context.Rotations
            .Include(rotation => rotation.RotationType)
            .Where(r =>
                (r.PgyYear == 1 || r.PgyYear == 2)
                && r.AcademicYear == copyRequest.FromAcademicYear
            )
            .AsNoTracking()
            .ToListAsync();

        if (rotations.Count == 0)
        {
            return BadRequest(
                GenericResponse.Failure(
                    $"Academic year {copyRequest.FromAcademicYear} is not copyable."
                )
            );
        }

        bool alreadyCopied = await context.Rotations
            .Where(r =>
                (r.PgyYear == 1 || r.PgyYear == 2)
                && r.AcademicYear == copyRequest.ToAcademicYear
            )
            .AnyAsync();

        if (alreadyCopied)
        {
            return BadRequest(
                GenericResponse.Failure(
                    $"Academic year {copyRequest.ToAcademicYear} is already copied."
                )
            );
        }

        Dictionary<Guid, Guid> newGuidMappings = [];
        foreach (Rotation rotation in rotations)
        {
            newGuidMappings.TryAdd(rotation.RotationId, Guid.NewGuid());
            rotation.RotationId = newGuidMappings[rotation.RotationId];
            rotation.ResidentId = null;
            rotation.Pgy4RotationScheduleId = null;
            rotation.AcademicYear = copyRequest.ToAcademicYear;
            context.Rotations.Add(rotation);
        }

        await context.SaveChangesAsync();
        return Ok(rotations.Select(rotationConverter.CreateRotationResponseFromModel));
    }

    // GET: /api/rotations/types
    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<RotationTypeResponse>>> GetRotationTypes(
        [FromQuery] IList<int>? pgyYear
    )
    {
        IQueryable<RotationType> rotationTypes = context.RotationTypes.AsQueryable();

        if (pgyYear is { Count: > 0 })
        {
            PgyYearFlags flags = PgyYearFlagsExtensions.FromYears(pgyYear);
            rotationTypes = rotationTypes.Where(
                r => (r.PgyYearFlags & flags) != 0
            );
        }

        List<RotationType> rotationTypesList = await rotationTypes.ToListAsync();
        return Ok(rotationTypesList.Select(rotationTypeConverter.CreateRotationTypeResponse));
    }
}