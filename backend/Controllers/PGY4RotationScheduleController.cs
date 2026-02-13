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

    [HttpGet("generate-one")]
    public async Task<ActionResult<PGY4RotationScheduleResponse>> GenerateOneSchedule()
    {
        int seed = 12;

        // Find all resident whose graduate year is 3
        List<RotationPrefRequest> rotationPrefRequests =
        [
            .. IncludeAllRotationPrefRequestProperties(context.RotationPrefRequests)
                .Include(request => request.Resident)
                .Where(request => request.Resident.GraduateYr == 3),
        ];

        // Convert all rotationPrefREquest models to the special algorithm type: ALgorithmRotationPrefRequest
        AlgorithmRotationPrefRequest[] algorithmPrefRequests =
        [
            .. rotationPrefRequests.Select(
                RotationPrefRequestConverter.CreateAlgorithmSchedulePrefRequestFromModel
            ),
        ];

        if (algorithmPrefRequests.Length == 0)
        {
            ModelState.AddModelError("None Found", "0 Rotation Preference Requests found!");
            return BadRequest(ModelState);
        }

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

        // Insert schedule
        Guid newScheduleId = Guid.NewGuid();
        PGY4RotationSchedule schedule = new()
        {
            PGY4RotationScheduleId = newScheduleId,
            Seed = seed.ToString(),
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
                int monthIndex = (i - monthOffset + 12) % 12;
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

        // Convert to response
        return Ok(null);
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
}
