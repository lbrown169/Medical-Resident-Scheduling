using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Pgy4Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class Pgy4RotationScheduleService(
    MedicalContext context,
    RotationConverter rotationConverter
)
{
    private readonly MedicalContext context = context;
    private readonly RotationConverter rotationConverter = rotationConverter;

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

    public IQueryable<RotationPrefRequest> IncludeAllRotationPrefRequestProperties(
        DbSet<RotationPrefRequest> rotationPrefRequestDbSet
    )
    {
        return rotationPrefRequestDbSet
            .Include(r => r.Resident)
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

    public async Task<List<Rotation>> GetRotationsFromGeneratedSchedule(
        Pgy4ScheduleData generatedSchedule,
        Guid newScheduleId
    )
    {
        Dictionary<string, RotationType> allRotationTypesDictionary = (
            await context.RotationTypes.ToListAsync()
        ).ToDictionary(keySelector: (rotationType) => rotationType.RotationName);

        List<Rotation> rotationsToAdd = [];

        foreach (KeyValuePair<Resident, Pgy4RotationTypeEnum[]> kvp in generatedSchedule.Schedule)
        {
            for (int calenderMonthIndex = 0; calenderMonthIndex < kvp.Value.Length; calenderMonthIndex++)
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
}