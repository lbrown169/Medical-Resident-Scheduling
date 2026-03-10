using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<Pgy4RotationSchedule> IncludeRotationTypeAndResidentProperties(
        this DbSet<Pgy4RotationSchedule> scheduleDbSet
    )
    {
        return scheduleDbSet
            .Include(s => s.Rotations)
                .ThenInclude((r) => r.RotationType)
            .Include((s) => s.Rotations)
                .ThenInclude((r) => r.Resident);
    }

    public static IQueryable<RotationPrefRequest> IncludeAllRotationPrefRequestProperties(
        this DbSet<RotationPrefRequest> rotationPrefRequestDbSet
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
}