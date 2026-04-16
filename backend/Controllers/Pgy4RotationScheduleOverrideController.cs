using MedicalDemo.Converters;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/pgy4-rotation-schedule-override")]
public class Pgy4RotationScheduleOverrideController(
    MedicalContext context,
    Pgy4RotationScheduleConverter pgy4RotationScheduleConverter,
    Pgy4RotationScheduleOverrideConverter pgy4RotationScheduleOverrideConverter
) : ControllerBase
{
    private readonly MedicalContext context = context;
    private readonly Pgy4RotationScheduleConverter pgy4RotationScheduleConverter =
        pgy4RotationScheduleConverter;
    private readonly Pgy4RotationScheduleOverrideConverter pgy4RotationScheduleOverrideConverter =
        pgy4RotationScheduleOverrideConverter;

    [HttpPost("{scheduleId}")]
    public async Task<ActionResult<Pgy4RotationScheduleOverrideResponse>> AddScheduleOverride(
        [FromRoute] Guid scheduleId,
        [FromBody] Pgy4RotationScheduleOverrideRequest request
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check schedule existence
        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == scheduleId);

        if (foundSchedule == null)
        {
            return NotFound("Schedule not found!");
        }

        // Check request ResidentId existence
        Resident? foundResident = await context.Residents.FirstOrDefaultAsync(
            (r) => r.ResidentId == request.ResidentId
        );

        if (foundResident == null)
        {
            return NotFound("Resident not found!");
        }

        if (foundResident.GraduateYr < 3)
        {
            ModelState.AddModelError(
                "Non-PGY3 Resident",
                "The resident ID passed in is not a PGY3 resident"
            );
            return BadRequest(ModelState);
        }

        // Check request rotationType existence
        RotationType? foundRotationType = await context.RotationTypes.FirstOrDefaultAsync(
            (r) => r.RotationTypeId == request.newRotationTypeId
        );

        if (foundRotationType == null)
        {
            return NotFound("Rotation Type not found!");
        }

        // Check if another override already exists, delete it if it does
        Pgy4RotationScheduleOverride? existingOverride =
            await context.Pgy4RotationScheduleOverrides.FirstOrDefaultAsync(
                (
                    o =>
                        o.ResidentOverrideId == request.ResidentId
                        && o.Pgy4RotationScheduleId == scheduleId
                        && o.RotationMonthOfYearOverride
                            == MonthOfYearExtensions.FromAcademicIndex(
                                request.AcademicMonthIndex,
                                false
                            )
                )
            );

        if (existingOverride != null)
        {
            context.Pgy4RotationScheduleOverrides.Remove(existingOverride);
            await context.SaveChangesAsync();
        }

        // Add to DB
        Pgy4RotationScheduleOverride newOverride =
            pgy4RotationScheduleOverrideConverter.CreateModelFromRequest(scheduleId, request);

        await context.Pgy4RotationScheduleOverrides.AddAsync(newOverride);
        await context.SaveChangesAsync();

        // Get Response
        newOverride = await context
            .Pgy4RotationScheduleOverrides.Include(o => o.RotationType)
            .Include(o => o.Resident)
            .FirstAsync(o =>
                o.Pgy4RotationScheduleOverrideId == newOverride.Pgy4RotationScheduleOverrideId
            );

        Pgy4RotationScheduleOverrideResponse response =
            pgy4RotationScheduleOverrideConverter.CreateResponseFromModel(newOverride);

        return Ok(response);
    }

    [HttpGet("{scheduleId}")]
    public async Task<
        ActionResult<Pgy4RotationScheduleOverrideListResponse>
    > GetAllOverridesByScheduleId([FromRoute] Guid scheduleId)
    {
        // Check schedule existence
        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .AsSplitQuery()
            .IncludeOverrides()
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == scheduleId);

        if (foundSchedule == null)
        {
            return NotFound("Schedule not found!");
        }

        Pgy4RotationScheduleOverrideListResponse response =
            pgy4RotationScheduleOverrideConverter.CreateListResponseFromModels(foundSchedule);

        return Ok(response);
    }

    [HttpDelete("{scheduleId}")]
    public async Task<ActionResult> DeleteOverrideByScheduleId(
        [FromRoute] Guid scheduleId,
        [FromBody] Pgy4RotationScheduleOverrideDeleteRequest deleteRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Delete from DB
        Pgy4RotationScheduleOverride? existingOverride =
            await context.Pgy4RotationScheduleOverrides.FirstOrDefaultAsync(
                (
                    o =>
                        o.ResidentOverrideId == deleteRequest.ResidentId
                        && o.Pgy4RotationScheduleId == scheduleId
                        && o.RotationMonthOfYearOverride
                            == MonthOfYearExtensions.FromAcademicIndex(
                                deleteRequest.AcademicMonthIndex,
                                false
                            )
                )
            );

        if (existingOverride == null)
        {
            return NotFound("Cannot find the corresponding override.");
        }

        context.Pgy4RotationScheduleOverrides.Remove(existingOverride);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{scheduleId}/all")]
    public async Task<ActionResult> DeleteAllOverridesByScheduleId([FromRoute] Guid scheduleId)
    {
        // Check schedule existence
        Pgy4RotationSchedule? foundSchedule =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (s) => s.Pgy4RotationScheduleId == scheduleId
            );

        if (foundSchedule == null)
        {
            return NotFound("Schedule not found!");
        }

        // Delete overrides from DB
        List<Pgy4RotationScheduleOverride> overrides = await context
            .Pgy4RotationScheduleOverrides.Where((o) => o.Pgy4RotationScheduleId == scheduleId)
            .ToListAsync();

        context.Pgy4RotationScheduleOverrides.RemoveRange(overrides);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{scheduleId}/apply-overrides")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> ApplyScheduleOverridesById(
        [FromRoute] Guid scheduleId
    )
    {
        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == scheduleId);

        if (foundSchedule == null)
        {
            return NotFound();
        }

        List<Pgy4RotationScheduleOverride>? overrides = null;

        overrides = await context
            .Pgy4RotationScheduleOverrides.Include(o => o.RotationType)
            .Where((o) => o.Pgy4RotationScheduleId == scheduleId)
            .ToListAsync();

        // Update schedule rotations with appropriate overrides
        pgy4RotationScheduleOverrideConverter.UpdateScheduleWithOverrides(foundSchedule, overrides);
        // Delete overrides from DB
        context.Pgy4RotationScheduleOverrides.RemoveRange(overrides);
        await context.SaveChangesAsync();

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

        return Ok(response);
    }
}