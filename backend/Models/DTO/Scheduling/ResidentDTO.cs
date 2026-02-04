namespace MedicalDemo.Models.DTO.Scheduling;

public abstract class ResidentDTO
{
    public string ResidentId { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public bool InTraining { get; set; }
    public HashSet<DateOnly> VacationRequests { get; set; } = new();
    public HashSet<DateOnly> WorkDays { get; set; } = new();
    public HashSet<DateOnly> CommitedWorkDays { get; set; } = new();

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
        foreach (DateOnly day in WorkDays)
        {
            CommitedWorkDays.Add(day);
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
}