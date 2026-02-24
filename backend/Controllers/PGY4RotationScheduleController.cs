using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator;
using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/pgy4-rotation-schedule")]
public class Pgy4RotatoinScheduleController(
    MedicalContext context,
    Pgy4RotationScheduleConverter pgy4RotationScheduleConverter,
    RotationPrefRequestConverter rotationPrefRequestConverter,
    ResidentConverter residentConverter,
    Pgy4RotationScheduleService pgy4RotationScheduleService,
    Pgy4RotationScheduleGenerator scheduleGenerator
) : ControllerBase
{
    private readonly MedicalContext context = context;
    private readonly Pgy4RotationScheduleConverter pgy4RotationScheduleConverter =
        pgy4RotationScheduleConverter;
    private readonly RotationPrefRequestConverter rotationPrefRequestConverter =
        rotationPrefRequestConverter;
    private readonly ResidentConverter residentConverter = residentConverter;
    private readonly Pgy4RotationScheduleService pgy4RotationScheduleService =
        pgy4RotationScheduleService;
    private readonly Pgy4RotationScheduleGenerator scheduleGenerator = scheduleGenerator;

    [HttpGet("{id}")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> GetScheduleById(
        [FromRoute] Guid id
    )
    {
        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.Include(s => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == id);

        if (foundSchedule == null)
        {
            return NotFound();
        }

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<Pgy4RotationSchedulesListResponse[]>> GetAllSchedules()
    {
        List<Pgy4RotationSchedule> foundSchedule = await context
            .Pgy4RotationSchedules.Include((s) => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
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
        int existingScheduleCount = await GetScheduleCount();

        if (existingScheduleCount + count > maxScheduleCount)
        {
            ModelState.AddModelError(
                "Schedule Count Error",
                $"Max schedule count of 5 will be exceeded. Current Schedule Count: {existingScheduleCount}"
            );
            return BadRequest(ModelState);
        }

        List<Resident> unsubmittedResidents = await ValidateAllPrefRequestSubmitted();
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
        List<RotationPrefRequest> rotationPrefRequests = await GetAllPGY3RotationPrefRequests();

        if (rotationPrefRequests.Count == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

        List<Pgy4RotationScheduleResponse> scheduleResponses = [];

        foreach (int seed in seeds)
        {
            // Generate a schedule
            Pgy4ScheduleData? generatedSchedule = GenerateSchedule(
                seed,
                rotationPrefRequests
            );

            if (generatedSchedule == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // Add generated schedule to DB
            Pgy4RotationSchedule rotationSchedule = await AddScheduleToDb(seed, generatedSchedule);
            Pgy4RotationScheduleResponse scheduleResponse = await GetScheduleResponseById(
                rotationSchedule.Pgy4RotationScheduleId
            );

            // Convert to response
            scheduleResponses.Add(scheduleResponse);
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
        int scheduleYear = GetScheduleYear();

        Pgy4RotationSchedule? scheduleToBePublished =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Pgy4RotationScheduleId == id
            );

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
                (schedule) => schedule.Year == scheduleYear
            );
        existingPublishedSchedule?.IsPublished = false;

        scheduleToBePublished.IsPublished = true;

        await context.SaveChangesAsync();

        Pgy4RotationScheduleResponse response = await GetScheduleResponseById(
            scheduleToBePublished.Pgy4RotationScheduleId
        );

        return Ok(response);
    }

    [HttpGet("published")]
    public async Task<ActionResult<Pgy4RotationScheduleResponse>> GetPublishedSchedule()
    {
        Pgy4RotationSchedule? foundSchedule =
            await context.Pgy4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Year == GetScheduleYear() && schedule.IsPublished
            );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        Pgy4RotationScheduleResponse response = await GetScheduleResponseById(
            foundSchedule.Pgy4RotationScheduleId
        );
        return response;
    }

    [HttpGet("resident/{id}")]
    public async Task<ActionResult<Pgy4ResidenRotationScheduleResponse>> GetScheduleByResident(
        [FromRoute] string id
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

        Pgy4RotationSchedule? foundSchedule = await context
            .Pgy4RotationSchedules.Include((schedule) => schedule.Rotations)
                .ThenInclude((r) => r.RotationType)
            .FirstOrDefaultAsync(
                (schedule) => schedule.Year == GetScheduleYear() && schedule.IsPublished
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

        if (residentRotations.Count == 0)
        {
            return NotFound("No schedule for this resident is found");
        }

        // Convert to response and return
        Pgy4ResidenRotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateSingleResidentRotationSchedule(
                foundResident,
                residentRotations
            );
        return Ok(response);
    }

    private async Task<List<RotationPrefRequest>> GetAllPGY3RotationPrefRequests()
    {
        List<RotationPrefRequest> rotationPrefRequests =
        [
            .. await pgy4RotationScheduleService
                .IncludeAllRotationPrefRequestProperties(context.RotationPrefRequests)
                .Include(request => request.Resident)
                .Where(request => request.Resident.GraduateYr == 3)
                .ToListAsync(),
        ];
        return rotationPrefRequests;
    }

    private Pgy4ScheduleData? GenerateSchedule(
        int seed,
        List<RotationPrefRequest> rotationPrefRequests
    )
    {
        // Convert all rotationPrefRequest models to the special algorithm type: ALgorithmRotationPrefRequest
        AlgorithmRotationPrefRequest[] algorithmPrefRequests =
        [
            .. rotationPrefRequests.Select(
                rotationPrefRequestConverter.CreateAlgorithmSchedulePrefRequestFromModel
            ),
        ];
        // Populate constraints
        IConstraint[] constraints =
        [
            new HasChiefRotationConstraint(),
            new InpatientConsultInJulyAndJanConstraint(),
            new Min2ConsultsInpatientConstraint(),
            new OneIopForenCommAddictPerMonthConstraint(),
        ];

        // Generate schedule
        scheduleGenerator.Initialize(algorithmPrefRequests, constraints, seed);
        scheduleGenerator.GenerateSchedule();
        Pgy4ScheduleData? generatedSchedule =
            scheduleGenerator.RotationSchedule;

        return generatedSchedule;
    }

    private async Task<Pgy4RotationSchedule> AddScheduleToDb(
        int seed,
        Pgy4ScheduleData generatedSchedule
    )
    {
        // Insert schedule
        Guid newScheduleId = Guid.NewGuid();

        Pgy4RotationSchedule schedule = new()
        {
            Pgy4RotationScheduleId = newScheduleId,
            Seed = seed,
            Year = GetScheduleYear(),
            IsPublished = false,
        };

        await context.Pgy4RotationSchedules.AddAsync(schedule);
        await context.SaveChangesAsync();

        // Add result to rotations table
        List<Rotation> rotationsToAdd =
            await pgy4RotationScheduleService.GetRotationsFromGeneratedSchedule(
                generatedSchedule,
                newScheduleId
            );

        // Insert rotations
        await context.Rotations.AddRangeAsync(rotationsToAdd);
        await context.SaveChangesAsync();

        return schedule;
    }

    private async Task<Pgy4RotationScheduleResponse> GetScheduleResponseById(Guid scheduleId)
    {
        Pgy4RotationSchedule addedSchedule = await context
            .Pgy4RotationSchedules.Include(s => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
            .FirstAsync((s) => s.Pgy4RotationScheduleId == scheduleId);

        Pgy4RotationScheduleResponse response =
            pgy4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(addedSchedule);
        return response;
    }

    private async Task<List<Resident>> ValidateAllPrefRequestSubmitted()
    {
        List<Resident> residents = await context
            .Residents.Where((resident) => resident.GraduateYr == 3)
            .ToListAsync();
        List<RotationPrefRequest> requests = await context.RotationPrefRequests.ToListAsync();

        List<Resident> unsubmittedResidents = [];

        foreach (Resident resident in residents)
        {
            RotationPrefRequest? foundRequest = requests.FirstOrDefault(
                (request) => request.ResidentId == resident.ResidentId
            );
            if (foundRequest == null)
            {
                unsubmittedResidents.Add(resident);
            }
        }

        return unsubmittedResidents;
    }

    private async Task<int> GetScheduleCount()
    {
        int count = (await context.Pgy4RotationSchedules.ToListAsync()).Count;
        return count;
    }

    private static int GetScheduleYear()
    {
        int currentYear = DateTime.Today.Year;
        int currentMonth = DateTime.Today.Month;

        int scheduleYear = currentYear;
        if (currentMonth < 7)
        {
            scheduleYear--;
        }

        return scheduleYear;
    }
}