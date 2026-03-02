using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator;
using MedicalDemo.Algorithms.Pgy4RotationScheduleGenerator.Constraints;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class Pgy4RotationScheduleService(
    MedicalContext context,
    RotationConverter rotationConverter,
    RotationPrefRequestConverter rotationPrefRequestConverter,
    Pgy4RotationScheduleGenerator scheduleGenerator
)
{
    private readonly MedicalContext context = context;
    private readonly RotationConverter rotationConverter = rotationConverter;
    private readonly RotationPrefRequestConverter rotationPrefRequestConverter =
        rotationPrefRequestConverter;
    private readonly Pgy4RotationScheduleGenerator scheduleGenerator = scheduleGenerator;

    public int[] GenerateSeeds(int count)
    {
        HashSet<int> seeds = [];
        Random random = new();
        while (seeds.Count != count)
        {
            seeds.Add(random.Next(int.MaxValue));
        }

        return [.. seeds];
    }

    public async Task<List<Rotation>> GetRotationsFromGeneratedSchedule(
        Pgy4ScheduleData generatedSchedule,
        Guid newScheduleId
    )
    {
        Dictionary<string, RotationType> allRotationTypesDictionary = (
            await context.RotationTypes.ToListAsync()
        ).ToDictionary(keySelector: (rotationType) => rotationType.RotationName);

        List<Rotation> rotationsToAdd = [];

        foreach (KeyValuePair<string, Pgy4RotationTypeEnum[]> kvp in generatedSchedule.Schedule)
        {
            for (
                int calenderMonthIndex = 0;
                calenderMonthIndex < kvp.Value.Length;
                calenderMonthIndex++
            )
            {
                Pgy4RotationTypeEnum typeEnum = kvp.Value[calenderMonthIndex];
                string rotationName = typeEnum.GetDisplayName();
                RotationType currentRotationType = allRotationTypesDictionary[rotationName];

                Rotation newRotation = rotationConverter.CreateRotationFromResidentAndType(
                    kvp.Key,
                    currentRotationType,
                    newScheduleId,
                    (MonthOfYear)calenderMonthIndex
                );
                rotationsToAdd.Add(newRotation);
            }
        }

        return rotationsToAdd;
    }

    public async Task<List<RotationPrefRequest>> GetAllPgy3RotationPrefRequests()
    {
        return await context.RotationPrefRequests.IncludeAllRotationPrefRequestProperties()
            .Include(request => request.Resident)
            .Where(request => request.Resident.GraduateYr == 3)
            .ToListAsync();
    }

    public Pgy4ScheduleData? GenerateSchedule(
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
        Pgy4ScheduleData? generatedSchedule = scheduleGenerator.RotationSchedule;

        return generatedSchedule;
    }

    public async Task<Pgy4RotationSchedule> AddScheduleToDb(
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
        List<Rotation> rotationsToAdd = await GetRotationsFromGeneratedSchedule(
            generatedSchedule,
            newScheduleId
        );

        // Insert rotations
        await context.Rotations.AddRangeAsync(rotationsToAdd);
        await context.SaveChangesAsync();

        return schedule;
    }

    public async Task<Pgy4RotationSchedule?> GetScheduleById(Guid scheduleId)
    {
        return await context
            .Pgy4RotationSchedules.IncludeRotationTypeAndResidentProperties()
            .FirstOrDefaultAsync((s) => s.Pgy4RotationScheduleId == scheduleId);
    }

    public async Task<List<Resident>> ValidateAllPrefRequestSubmitted()
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

    public async Task<int> GetScheduleCount()
    {
        int count = await context.Pgy4RotationSchedules.CountAsync();
        return count;
    }

    public int GetScheduleYear()
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