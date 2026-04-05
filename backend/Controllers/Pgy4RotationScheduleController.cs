using MedicalDemo.Converters;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/pgy4-rotation-schedule")]
public class Pgy4RotationScheduleController(
    MedicalContext context,
    Pgy4RotationScheduleConverter pgy4RotationScheduleConverter,
    ResidentConverter residentConverter,
    Pgy4RotationScheduleService pgy4RotationScheduleService,
    Pgy4RotationScheduleOverrideConverter pgy4RotationScheduleOverrideConverter
) : ControllerBase
{
    private readonly MedicalContext context = context;
    private readonly Pgy4RotationScheduleConverter pgy4RotationScheduleConverter =
        pgy4RotationScheduleConverter;
    private readonly ResidentConverter residentConverter = residentConverter;
    private readonly Pgy4RotationScheduleService pgy4RotationScheduleService =
        pgy4RotationScheduleService;
    private readonly Pgy4RotationScheduleOverrideConverter pgy4RotationScheduleOverrideConverter =
        pgy4RotationScheduleOverrideConverter;

    [HttpGet("{scheduleId}")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> GetScheduleById(
        [FromRoute] Guid scheduleId,
        [FromQuery] bool applyOverrides = false
    )
    {
        Pgy4RotationSchedule? foundSchedule = await pgy4RotationScheduleService.GetScheduleById(
            scheduleId
        );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        List<Pgy4RotationScheduleOverride>? overrides = null;

        if (applyOverrides)
        {
            overrides = await context
                .Pgy4RotationScheduleOverrides.Include(o => o.RotationType)
                .Where((o) => o.Pgy4RotationScheduleId == scheduleId)
                .ToListAsync();

            pgy4RotationScheduleOverrideConverter.UpdateScheduleWithOverrides(
                foundSchedule,
                overrides
            );
        }

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<Pgy4RotationSchedulesListResponse[]>> GetAllSchedules()
    {
        List<Pgy4RotationSchedule> foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .ToListAsync();

        Pgy4RotationSchedulesListResponse response =
            pgy4RotationScheduleConverter.CreateRotationSchedulesListResponseFromModel(
                foundSchedule
            );

        return Ok(response);
    }

    [HttpGet("generate")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> GenerateSchedules(
        [FromQuery] int count = 1
    )
    {
        if (count <= 0)
        {
            ModelState.AddModelError(
                "Invalid Schedule Count",
                "The schedule count cannot be 0 or less"
            );
            return BadRequest(ModelState);
        }

        const int maxScheduleCount = 5;
        int existingScheduleCount = await pgy4RotationScheduleService.GetScheduleCount();

        if (existingScheduleCount + count > maxScheduleCount)
        {
            ModelState.AddModelError(
                "Schedule Count Error",
                $"Max schedule count of 5 will be exceeded. Current Schedule Count: {existingScheduleCount}"
            );
            return BadRequest(ModelState);
        }

        List<Resident> unsubmittedResidents =
            await pgy4RotationScheduleService.ValidateAllPrefRequestSubmitted();
        if (unsubmittedResidents.Count != 0)
        {
            UnsubmittedResidentsResponse errorResponse = new()
            {
                Message = "Unsubmitted Resident Requests",
                UnsubmittedResidents =
                [
                    .. unsubmittedResidents.Select(
                        residentConverter.CreateResidentResponseFromResident
                    ),
                ],
            };

            return BadRequest(errorResponse);
        }

        int[] seeds = pgy4RotationScheduleService.GenerateSeeds(count);

        // Find all resident whose graduate year is 3
        List<RotationPrefRequest> rotationPrefRequests =
            await pgy4RotationScheduleService.GetAllPgy3RotationPrefRequests();

        if (rotationPrefRequests.Count == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

        List<Pgy4RotationScheduleResponse> scheduleResponses = [];

        foreach (int seed in seeds)
        {
            // Generate a schedule
            Pgy4ScheduleData? generatedSchedule = pgy4RotationScheduleService.GenerateSchedule(
                seed,
                rotationPrefRequests
            );

            if (generatedSchedule == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // Add generated schedule to DB
            Pgy4RotationSchedule rotationSchedule =
                await pgy4RotationScheduleService.AddScheduleToDb(seed, generatedSchedule);
            Pgy4RotationSchedule? addedSchedule = await pgy4RotationScheduleService.GetScheduleById(
                rotationSchedule.Pgy4RotationScheduleId
            );

            // Convert to response
            if (addedSchedule != null)
            {
                Pgy4RotationScheduleResponse response =
                    pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(
                        addedSchedule
                    );
                scheduleResponses.Add(response);
            }
        }

        Pgy4RotationSchedulesListResponse listResponse = new()
        {
            Count = scheduleResponses.Count,
            Schedules = scheduleResponses,
        };

        return Ok(listResponse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([FromRoute] Guid id)
    {
        Pgy4RotationSchedule? foundSchedule =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (s) => s.Pgy4RotationScheduleId == id
            );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        context.Pgy4RotationSchedules.Remove(foundSchedule);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("publish/{id}")]
    public async Task<ActionResult> PublishSchedule([FromRoute] Guid id)
    {
        int scheduleYear = pgy4RotationScheduleService.GetAcademicYear();

        Pgy4RotationSchedule? scheduleToBePublished =
            await pgy4RotationScheduleService.GetScheduleById(id);
        ;

        if (scheduleToBePublished == null)
        {
            return NotFound();
        }
        else if (scheduleToBePublished.Year != scheduleYear)
        {
            ModelState.AddModelError(
                "Schedule Year Mismatch",
                "Cannot publish schedule not in the current academic year."
            );
            return BadRequest(ModelState);
        }

        Pgy4RotationSchedule? existingPublishedSchedule =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Year == scheduleYear && schedule.IsPublished
            );

        existingPublishedSchedule?.IsPublished = false;
        scheduleToBePublished.IsPublished = true;

        await context.SaveChangesAsync();

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(
                scheduleToBePublished
            );

        return Ok(response);
    }

    [HttpPatch("unpublish")]
    public async Task<ActionResult> UnpublishSchedule()
    {
        int scheduleYear = pgy4RotationScheduleService.GetAcademicYear();

        Pgy4RotationSchedule? foundPublishedSchedule =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Year == scheduleYear && schedule.IsPublished
            );

        if (foundPublishedSchedule == null)
        {
            return NotFound("No schedule has been published.");
        }

        foundPublishedSchedule.IsPublished = false;
        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("published")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> GetPublishedSchedule()
    {
        int academicYear = pgy4RotationScheduleService.GetAcademicYear();

        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync(
                (schedule) => schedule.Year == academicYear && schedule.IsPublished
            );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

        return response;
    }

    [HttpGet("resident/{id}")]
    public async Task<ActionResult<Pgy4ResidentRotationScheduleResponse>> GetScheduleByResident(
        [FromRoute] string id,
        [FromQuery] bool applyOverrides = false
    )
    {
        Resident? foundResident = await context.Residents.FirstOrDefaultAsync(
            (resident) => resident.ResidentId == id
        );

        // Check resident existence
        if (foundResident == null)
        {
            return NotFound();
        }

        // Check resident graduate year validity
        if (foundResident.GraduateYr != 3)
        {
            ModelState.AddModelError(
                "Non-PGY3 Resident",
                "The resident ID passed in is not a PGY3 resident"
            );
            return BadRequest(ModelState);
        }

        int academicYear = pgy4RotationScheduleService.GetAcademicYear();

        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync(
                (schedule) => schedule.Year == academicYear && schedule.IsPublished
            );

        // Check published schedule existence
        if (foundSchedule == null)
        {
            return NotFound("No schedule has been published");
        }

        // Get all resident rotation from the published schedule
        List<Rotation> residentRotations =
        [
            .. foundSchedule.Rotations.Where(
                (rotation) => rotation.ResidentId == foundResident.ResidentId
            ),
        ];

        // Apply overrides if needed
        List<Pgy4RotationScheduleOverride>? overrides = null;

        if (applyOverrides)
        {
            overrides = await context
                .Pgy4RotationScheduleOverrides.Include(o => o.RotationType)
                .Where(
                    (o) =>
                        o.Pgy4RotationScheduleId == foundSchedule.Pgy4RotationScheduleId
                        && o.ResidentOverrideId == foundResident.ResidentId
                )
                .ToListAsync();

            pgy4RotationScheduleOverrideConverter.UpdateResidentScheduleWithOverrides(
                residentRotations,
                overrides
            );
        }

        if (residentRotations.Count == 0)
        {
            return NotFound("No schedule for this resident is found");
        }

        // Convert to response and return
        Pgy4ResidentRotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateSingleResidentRotationSchedule(
                foundResident,
                residentRotations
            );
        return Ok(response);
    }

    [HttpGet("{scheduleId}/constraint-errors")]
    public async Task<
        ActionResult<Pgy4ScheduleConstraintViolationsListResponse>
    > GetScheduleConstraintErrorsById([FromRoute] Guid scheduleId)
    {
        // Get schedule from DB
        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == scheduleId);

        if (foundSchedule == null)
        {
            return NotFound();
        }

        List<Pgy4RotationScheduleOverride>? overrides = null;

        // Apply overrides to schedule
        overrides = await context
            .Pgy4RotationScheduleOverrides.Include(o => o.RotationType)
            .Where((o) => o.Pgy4RotationScheduleId == scheduleId)
            .ToListAsync();

        pgy4RotationScheduleOverrideConverter.UpdateScheduleWithOverrides(foundSchedule, overrides);

        Pgy4ScheduleData scheduleData =
            pgy4RotationScheduleConverter.CreateAlgorithmScheduleDataFromModel(foundSchedule);

        // Get violations
        List<Pgy4ConstraintViolation> violations =
            pgy4RotationScheduleService.GetConstraintViolations(scheduleData);

        // Convert to response
        Pgy4ScheduleConstraintViolationsListResponse violationResponse =
            pgy4RotationScheduleConverter.CreateViolationsListResponse(foundSchedule, violations);

        return Ok(violationResponse);
    }
}