using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotationScheduleGenerator.Algorithm;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/pgy4-rotation-schedule")]
public class PGY4RotatoinScheduleController(MedicalContext context) : ControllerBase
{
    private readonly MedicalContext context = context;

    [HttpGet("{id}")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GetScheduleById(
        [FromRoute] Guid id
    )
    {
        PGY4RotationSchedule? foundSchedule = await context
            .PGY4RotationSchedules.Include(s => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
            .FirstOrDefaultAsync((s) => s.PGY4RotationScheduleId == id);

        if (foundSchedule == null)
        {
            return NotFound();
        }

        PGY4RotationScheduleResponse response =
            PGY4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PGY4RotationSchedulesListResponse[]>> GetAllSchedules()
    {
        List<PGY4RotationSchedule> foundSchedule = await context
            .PGY4RotationSchedules.Include((s) => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
            .ToListAsync();

        PGY4RotationSchedulesListResponse response =
            PGY4RotationScheduleConverter.CreateRotationSchedulesListResponseFromModel(
                foundSchedule
            );

        return Ok(response);
    }

    [HttpGet("generate")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GenerateSchedules(
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
            return BadRequest(
                new
                {
                    Message = "Unsubmitted Resident Requests",
                    UnsubmittedResidents = unsubmittedResidents.Select(resident =>
                        new ResidentConverter().CreateResidentResponseFromResident(resident)
                    ),
                }
            );
        }

        HashSet<int> seeds = [];
        Random random = new();
        while (seeds.Count != count)
        {
            seeds.Add(random.Next(int.MaxValue));
        }

        // Find all resident whose graduate year is 3
        List<RotationPrefRequest> rotationPrefRequests = await GetAllPGY3RotationPrefRequests();

        if (rotationPrefRequests.Count == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

        List<PGY4RotationScheduleResponse> scheduleResponses = [];

        foreach (int seed in seeds)
        {
            // Generate a schedule
            Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule = GenerateSchedule(
                seed,
                rotationPrefRequests
            );

            // Add generated schedule to DB
            PGY4RotationSchedule rotationSchedule = await AddScheduleToDb(seed, generatedSchedule);
            PGY4RotationScheduleResponse scheduleResponse = await GetScheduleResponseById(
                rotationSchedule.PGY4RotationScheduleId
            );

            // Convert to response
            scheduleResponses.Add(scheduleResponse);
        }

        PGY4RotationSchedulesListResponse listResponse = new()
        {
            Count = scheduleResponses.Count,
            Schedules = scheduleResponses,
        };

        return Ok(listResponse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([FromRoute] Guid id)
    {
        PGY4RotationSchedule? foundSchedule =
            await context.PGY4RotationSchedules.FirstOrDefaultAsync(
                (s) => s.PGY4RotationScheduleId == id
            );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        context.PGY4RotationSchedules.Remove(foundSchedule);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("publish/{id}")]
    public async Task<ActionResult> PublishSchedule([FromRoute] Guid id)
    {
        int scheduleYear = GetScheduleYear();

        PGY4RotationSchedule? scheduleToBePublished =
            await context.PGY4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.PGY4RotationScheduleId == id
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

        PGY4RotationSchedule? existingPublishedSchedule =
            await context.PGY4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Year == scheduleYear
            );
        existingPublishedSchedule?.IsPublished = false;

        scheduleToBePublished.IsPublished = true;

        await context.SaveChangesAsync();

        PGY4RotationScheduleResponse response = await GetScheduleResponseById(
            scheduleToBePublished.PGY4RotationScheduleId
        );

        return Ok(response);
    }

    [HttpGet("published")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GetPublishedSchedule()
    {
        PGY4RotationSchedule? foundSchedule =
            await context.PGY4RotationSchedules.FirstOrDefaultAsync(
                (schedule) => schedule.Year == GetScheduleYear() && schedule.IsPublished
            );

        if (foundSchedule == null)
        {
            return NotFound();
        }

        PGY4RotationScheduleResponse response = await GetScheduleResponseById(
            foundSchedule.PGY4RotationScheduleId
        );
        return response;
    }

    [HttpGet("resident/{id}")]
    public async Task<ActionResult<PGY4ResidenRotationScheduleResponse>> GetScheduleByResident(
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

        PGY4RotationSchedule? foundSchedule = await context
            .PGY4RotationSchedules.Include((schedule) => schedule.Rotations)
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
        PGY4ResidenRotationScheduleResponse response =
            PGY4RotationScheduleConverter.CreateSingleResidentRotationSchedule(
                foundResident,
                residentRotations
            );
        return Ok(response);
    }

    private async Task<List<RotationPrefRequest>> GetAllPGY3RotationPrefRequests()
    {
        List<RotationPrefRequest> rotationPrefRequests =
        [
            .. IncludeAllRotationPrefRequestProperties(context.RotationPrefRequests)
                .Include(request => request.Resident)
                .Where(request => request.Resident.GraduateYr == 3),
        ];
        return rotationPrefRequests;
    }

    private static IQueryable<RotationPrefRequest> IncludeAllRotationPrefRequestProperties(
        DbSet<RotationPrefRequest> rotations
    )
    {
        return rotations
            .Include(r => r.FirstPriority)
            .Include(r => r.SecondPriority)
            .Include(r => r.ThirdPriority)
            .Include(r => r.FourthPriority)
            .Include(r => r.FifthPriority)
            .Include(r => r.SixthPriority)
            .Include(r => r.SeventhPriority)
            .Include(r => r.EighthPriority)
            .Include(r => r.FirstAlternative)
            .Include(r => r.SecondAlternative)
            .Include(r => r.ThirdAlternative)
            .Include(r => r.FirstAvoid)
            .Include(r => r.SecondAvoid)
            .Include(r => r.ThirdAvoid);
    }

    private static Dictionary<Resident, PGY4RotationTypeEnum?[]> GenerateSchedule(
        int seed,
        List<RotationPrefRequest> rotationPrefRequests
    )
    {
        // Convert all rotationPrefRequest models to the special algorithm type: ALgorithmRotationPrefRequest
        AlgorithmRotationPrefRequest[] algorithmPrefRequests =
        [
            .. rotationPrefRequests.Select(
                RotationPrefRequestConverter.CreateAlgorithmSchedulePrefRequestFromModel
            ),
        ];
        // Populate constraints
        IConstraintRule[] constraints =
        [
            new HasChiefRotation(),
            new InpatientConsultInJulyAndJan(),
            new Min2ConsultsInpatient(),
            new OneIopForensicCommunityAddictionPerMonth(),
        ];

        // Generate schedule
        PGY4RotationScheduleGenerator generator = new(algorithmPrefRequests, constraints, seed);
        generator.GenerateSchedule();
        Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule =
            generator.RotationSchedule;

        return generatedSchedule;
    }

    private async Task<PGY4RotationSchedule> AddScheduleToDb(
        int seed,
        Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule
    )
    {
        // Insert schedule
        Guid newScheduleId = Guid.NewGuid();

        PGY4RotationSchedule schedule = new()
        {
            PGY4RotationScheduleId = newScheduleId,
            Seed = seed,
            Year = GetScheduleYear(),
            IsPublished = false,
        };

        await context.PGY4RotationSchedules.AddAsync(schedule);
        await context.SaveChangesAsync();

        // Add result to rotations table
        Dictionary<string, RotationType> allRotationTypesDictionary = (
            await context.RotationTypes.ToListAsync()
        ).ToDictionary((rotationType) => rotationType.RotationName);
        List<Rotation> rotationsToAdd = [];

        foreach (KeyValuePair<Resident, PGY4RotationTypeEnum?[]> kvp in generatedSchedule)
        {
            for (int i = 0; i < kvp.Value.Length; i++)
            {
                PGY4RotationTypeEnum? typeEnum = kvp.Value[i];

                if (typeEnum == null)
                {
                    continue;
                }

                const int monthOffset = 6;
                const int totalMonths = 12;
                int monthIndex = (i - monthOffset + totalMonths) % totalMonths;
                string rotationName = RotationTypeConverter.ConvertRotationTypeEnumToName(
                    (PGY4RotationTypeEnum)typeEnum
                );
                RotationType currentRotationType = allRotationTypesDictionary[rotationName];

                Rotation newRotation = RotationConverter.CreateRotationFromResidentAndType(
                    kvp.Key,
                    currentRotationType,
                    newScheduleId,
                    monthIndex
                );
                rotationsToAdd.Add(newRotation);
            }
        }

        // Insert rotations
        await context.Rotations.AddRangeAsync(rotationsToAdd);
        await context.SaveChangesAsync();

        return schedule;
    }

    private async Task<PGY4RotationScheduleResponse> GetScheduleResponseById(Guid scheduleId)
    {
        PGY4RotationSchedule addedSchedule = await context
            .PGY4RotationSchedules.Include(s => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident)
            .FirstAsync((s) => s.PGY4RotationScheduleId == scheduleId);

        PGY4RotationScheduleResponse response =
            PGY4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(addedSchedule);
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
        int count = (await context.PGY4RotationSchedules.ToListAsync()).Count;
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
