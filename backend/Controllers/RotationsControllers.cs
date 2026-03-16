using MedicalDemo.Converters;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RotationsController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly RotationConverter _rotationConverter;
    private readonly RotationTypeConverter _rotationTypeConverter;

    public RotationsController(MedicalContext context, RotationConverter rotationConverter, RotationTypeConverter rotationTypeConverter)
    {
        _context = context;
        _rotationConverter = rotationConverter;
        _rotationTypeConverter = rotationTypeConverter;
    }

    // GET: api/rotations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> GetRotations(
        [FromQuery] int? pgyYear,
        [FromQuery] int? academicYear
    )
    {
        IQueryable<Rotation> query = _context.Rotations.AsQueryable();

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
            .Select(_rotationConverter.CreateRotationResponseFromModel)
            .ToList();

        return responses;
    }

    // PATCH: api/rotations/{id}
    [HttpPatch("{id}")]
    public async Task<ActionResult<IEnumerable<RotationResponse>>> UpdateRotation(Guid id,
        [FromBody] RotationUpdateRequest updateRequest)
    {
        List<Rotation> existingRotations = await _context.Rotations.Where(r => r.RotationId == id).ToListAsync();
        if (existingRotations.Count == 0)
        {
            return NotFound(GenericResponse.Failure("Rotation not found"));
        }

        // Update the fields
        existingRotations.ForEach(r => r.ResidentId = updateRequest.ResidentId);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingRotations.Select(_rotationConverter.CreateRotationResponseFromModel));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // PATCH: api/rotations/{id}/{calendarMonth}
    [HttpPatch("{id}/{calendarMonth}")]
    public async Task<ActionResult<RotationResponse>> UpdateRotationMonth(
        Guid id,
        int calendarMonth,
        [FromBody] RotationMonthUpdateRequest updateRequest
    )
    {
        Rotation? existingRotation = await _context.Rotations.FindAsync(id, MonthOfYearExtensions.FromCalendarIndex(calendarMonth, false));
        if (existingRotation == null)
        {
            return NotFound(GenericResponse.Failure("Rotation not found"));
        }

        existingRotation.RotationTypeId = updateRequest.RotationTypeId;
        await _context.SaveChangesAsync();
        return Ok(_rotationConverter.CreateRotationResponseFromModel(existingRotation));
    }

    // GET: api/rotations/copyable
    [HttpGet("copyable")]
    public async Task<ActionResult<IEnumerable<int>>> GetCopyableAcademicYears()
    {
        List<int> copyable = await _context.Rotations
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
        List<Rotation> rotations = await _context.Rotations
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

        bool alreadyCopied = await _context.Rotations
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
            _context.Rotations.Add(rotation);
        }

        await _context.SaveChangesAsync();
        return Ok(rotations.Select(_rotationConverter.CreateRotationResponseFromModel));
    }

    // GET: /api/rotations/types
    public async Task<ActionResult<IEnumerable<RotationTypeResponse>>> GetRotationTypes()
    {
        List<RotationType> rotationTypes = await _context.RotationTypes.ToListAsync();
        return Ok(rotationTypes.Select(_rotationTypeConverter.CreateRotationTypeResponse));
    }
}