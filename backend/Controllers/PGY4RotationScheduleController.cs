using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotationScheduleGenerator.Algorithm;

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

        PGY4RotationScheduleResponse response = PGY4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(foundSchedule);

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

    [HttpGet("generate-one")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GenerateOneSchedule()
    {
        int seed = new Random().Next(100);

        // Find all resident whose graduate year is 3
        List<RotationPrefRequest> rotationPrefRequests = await GetAllPGY3RotationPrefRequests();

        if (rotationPrefRequests.Count == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

        Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule = GenerateSchedule(seed, rotationPrefRequests);
        PGY4RotationSchedule rotationSchedule = await AddScheduleToDb(seed, generatedSchedule);
        PGY4RotationScheduleResponse scheduleResponse = await GetScheduleResponseById(rotationSchedule.PGY4RotationScheduleId);

        return Ok(scheduleResponse);
    }

    [HttpGet("generate-three")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GenerateSchedules()
    {
        List<int> seeds = [];
        Random random = new();
        seeds.Add(random.Next(100));
        seeds.Add(random.Next(100));
        seeds.Add(random.Next(100));

        // Find all resident whose graduate year is 3
        List<RotationPrefRequest> rotationPrefRequests = await GetAllPGY3RotationPrefRequests();

        if (rotationPrefRequests.Count == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

        List<PGY4RotationScheduleResponse> scheduleResponses = [];

        // For each seed
        // Generate a schedule
        // Add to DB
        // Convert to response
        foreach (int seed in seeds)
        {
            Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule = GenerateSchedule(seed, rotationPrefRequests);
            PGY4RotationSchedule rotationSchedule = await AddScheduleToDb(seed, generatedSchedule);
            PGY4RotationScheduleResponse scheduleResponse = await GetScheduleResponseById(rotationSchedule.PGY4RotationScheduleId);
            scheduleResponses.Add(scheduleResponse);
        }

        PGY4RotationSchedulesListResponse listResponse = new()
        {
            Count = scheduleResponses.Count,
            Schedules = scheduleResponses
        };

        return Ok(listResponse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([FromRoute] Guid id)
    {
        PGY4RotationSchedule? foundSchedule = await context
            .PGY4RotationSchedules
            .FirstOrDefaultAsync((s) => s.PGY4RotationScheduleId == id);

        if (foundSchedule == null)
        {
            return NotFound();
        }

        context.PGY4RotationSchedules.Remove(foundSchedule);
        await context.SaveChangesAsync();

        return NoContent();
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

    private static Dictionary<Resident, PGY4RotationTypeEnum?[]> GenerateSchedule(int seed, List<RotationPrefRequest> rotationPrefRequests)
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

    private async Task<PGY4RotationSchedule> AddScheduleToDb(int seed, Dictionary<Resident, PGY4RotationTypeEnum?[]> generatedSchedule)
    {
        // Insert schedule
        Guid newScheduleId = Guid.NewGuid();
        PGY4RotationSchedule schedule = new()
        {
            PGY4RotationScheduleId = newScheduleId,
            Seed = seed,
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

        PGY4RotationScheduleResponse response = PGY4RotationScheduleConverter.CreateRotationScheduleResponseFromModel(addedSchedule);
        return response;
    }
}
