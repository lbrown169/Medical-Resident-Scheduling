using System.Text;
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
    Pgy4RotationScheduleGenerator scheduleGenerator,
    HasChiefRotationConstraint hasChiefRotationConstraint,
    InpatientConsultInJulyAndJanConstraint inpatientConsultInJulyAndJanConstraint,
    Min2ConsultsInpatientConstraint min2ConsultsInpatientConstraint,
    OneIopForenCommAddictPerMonthConstraint oneIopForenCommAddictPerMonthConstraint
)
{
    private readonly MedicalContext context = context;
    private readonly RotationConverter rotationConverter = rotationConverter;
    private readonly RotationPrefRequestConverter rotationPrefRequestConverter =
        rotationPrefRequestConverter;
    private readonly Pgy4RotationScheduleGenerator scheduleGenerator = scheduleGenerator;

    private readonly HasChiefRotationConstraint hasChiefRotationConstraint =
        hasChiefRotationConstraint;
    private readonly InpatientConsultInJulyAndJanConstraint inpatientConsultInJulyAndJanConstraint =
        inpatientConsultInJulyAndJanConstraint;
    private readonly Min2ConsultsInpatientConstraint min2ConsultsInpatientConstraint =
        min2ConsultsInpatientConstraint;
    private readonly OneIopForenCommAddictPerMonthConstraint oneIopForenCommAddictPerMonthConstraint =
        oneIopForenCommAddictPerMonthConstraint;

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
        Guid newScheduleId,
        int academicYear
    )
    {
        Dictionary<string, RotationType> allRotationTypesDictionary = (
            await context.RotationTypes.ToListAsync()
        ).ToDictionary(keySelector: (rotationType) => rotationType.RotationName);

        List<Rotation> rotationsToAdd = [];

        foreach (
            KeyValuePair<
                AlgorithmResident,
                Pgy4RotationTypeEnum[]
            > kvp in generatedSchedule.Schedule
        )
        {
            Guid newRotationId = Guid.NewGuid();

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
                    kvp.Key.ResidentId,
                    currentRotationType,
                    newScheduleId,
                    newRotationId,
                    academicYear,
                    calenderMonthIndex
                );
                rotationsToAdd.Add(newRotation);
            }
        }

        return rotationsToAdd;
    }

    public async Task<List<RotationPrefRequest>> GetAllPgy3RotationPrefRequests()
    {
        return await context
            .RotationPrefRequests.IncludeAllRotationPrefRequestProperties()
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

        IRotationConstraint[] constraints =
        [
            hasChiefRotationConstraint,
            inpatientConsultInJulyAndJanConstraint,
            min2ConsultsInpatientConstraint,
            oneIopForenCommAddictPerMonthConstraint,
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

        int scheduleYear = GetAcademicYear();

        Pgy4RotationSchedule schedule = new()
        {
            Pgy4RotationScheduleId = newScheduleId,
            Seed = seed,
            Year = scheduleYear,
            IsPublished = false,
        };

        await context.Pgy4RotationSchedules.AddAsync(schedule);
        await context.SaveChangesAsync();

        // Add result to rotations table
        List<Rotation> rotationsToAdd = await GetRotationsFromGeneratedSchedule(
            generatedSchedule,
            newScheduleId,
            scheduleYear
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

    public int GetAcademicYear()
    {
        return DateOnly.FromDateTime(DateTime.Now).AcademicYear;
    }

    public string ExportScheduleCSVString(Pgy4RotationSchedule schedule)
    {
        StringBuilder builder = new();
        builder.AppendLine("Name,July,August,September,October,November,December,January,February,March,April,May,June");

        Dictionary<string, List<Rotation>> residentIdToRotations = [];

        foreach (Rotation rotation in schedule.Rotations)
        {
            if (residentIdToRotations.TryGetValue(rotation.ResidentId, out List<Rotation>? value))
            {
                value.Add(rotation);
            }
            else
            {
                residentIdToRotations.Add(rotation.ResidentId, []);
                residentIdToRotations[rotation.ResidentId].Add(rotation);
            }
        }

        foreach (KeyValuePair<string, List<Rotation>> kvp in residentIdToRotations)
        {
            List<string> orderedRotationsNames = [.. kvp.Value.OrderBy((r) => r.AcademicMonthIndex).Select((r) => r.RotationType.RotationName)];
            Resident resident = kvp.Value[0].Resident;

            builder.AppendLine($"{resident.FirstName} {resident.LastName}," + string.Join(",", orderedRotationsNames));
        }

        return builder.ToString();
    }

    public List<Pgy4ConstraintViolation> GetConstraintViolations(Pgy4ScheduleData scheduleData)
    {
        IRotationConstraint[] constraints =
        [
            hasChiefRotationConstraint,
            inpatientConsultInJulyAndJanConstraint,
            min2ConsultsInpatientConstraint,
            oneIopForenCommAddictPerMonthConstraint,
        ];

        List<Pgy4ConstraintViolation> violations = [];
        foreach (IRotationConstraint constraint in constraints)
        {
            violations.Add(
                constraint.GetRotationScheduleConstraintViolations(scheduleData.Schedule)
            );
        }

        return violations;
    }
}