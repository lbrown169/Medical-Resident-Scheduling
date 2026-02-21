namespace MedicalDemo.Models.DTO.Scheduling;

public abstract class ResidentDTO
{
    public string ResidentId { get; set; }
    public string Name { get; set; }
    public bool InTraining { get; set; }
    public HashSet<DateOnly> VacationRequests { get; set; } = new();
    public HashSet<DateOnly> WorkDays { get; set; } = new();
    public HashSet<DateOnly> CommitedWorkDays { get; set; } = new();
    public HashSet<DateOnly> PendingSaveWorkDays { get; set; } = new();
    public HashSet<DateOnly> AllPendingWorkDays => [.. PendingSaveWorkDays, .. WorkDays];

    public HospitalRole?[] RolePerMonth { get; set; } = new HospitalRole?[12];

    public abstract bool CanWork(DateOnly date);
    public abstract void AddWorkDay(DateOnly date);
    public abstract void RemoveWorkDay(DateOnly date);

    public bool CommitedWorkDay(DateOnly curDay)
    {
        return CommitedWorkDays.Contains(curDay);
    }

    public void SaveWorkDays()
    {
        for (int i = WorkDays.Count - 1; i >= 0; i--)
        {
            DateOnly day = WorkDays.ElementAt(i);
            CommitedWorkDays.Add(day);
            PendingSaveWorkDays.Add(day);
            WorkDays.Remove(day);
        }
    }

    public DateOnly LastWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Max()
            : new DateOnly(1, 1, 1);
    }

    public DateOnly FirstWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Min()
            : new DateOnly(9999, 12, 31);
    }

    public bool IsVacation(DateOnly curDay)
    {
        return VacationRequests.Contains(curDay);
    }

    public bool IsWorking(DateOnly curDay)
    {
        return WorkDays.Contains(curDay);
    }

    public bool CanAddWorkDay(DateOnly curDay)
    {
        return CanWork(curDay) && !IsWorking(curDay);
    }

    /// <summary>
    ///     Get hospital role for a resident for a month.
    /// </summary>
    /// <param name="index">Academic year indexed, July is 0.</param>
    /// <returns></returns>
    public HospitalRole GetHospitalRoleForMonth(int index)
    {
        if (RolePerMonth is null || RolePerMonth.Length < index + 1)
        {
            return HospitalRole.Unassigned;
        }
        return RolePerMonth[index] ?? HospitalRole.Unassigned;
    }
}