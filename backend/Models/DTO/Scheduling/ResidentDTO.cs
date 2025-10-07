namespace MedicalDemo.Models.DTO.Scheduling;

public abstract class ResidentDTO
{
    public string ResidentId { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public bool InTraining { get; set; }
    public HashSet<DateTime> VacationRequests { get; set; } = new();
    public HashSet<DateTime> WorkDays { get; set; } = new();
    public HashSet<DateTime> CommitedWorkDays { get; set; } = new();

    public HospitalRole[] RolePerMonth { get; set; } = new HospitalRole[12];

    public abstract bool CanWork(DateTime date);
    public abstract void AddWorkDay(DateTime date);
    public abstract void RemoveWorkDay(DateTime date);

    public bool CommitedWorkDay(DateTime curDay)
    {
        return CommitedWorkDays.Contains(curDay);
    }

    public void SaveWorkDays()
    {
        foreach (DateTime day in WorkDays)
        {
            CommitedWorkDays.Add(day);
        }
    }

    public DateTime LastWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Max()
            : new DateTime(1, 1, 1);
    }

    public DateTime FirstWorkDay()
    {
        return WorkDays.Count > 0
            ? WorkDays.Min()
            : new DateTime(9999, 12, 31);
    }

    public bool IsVacation(DateTime curDay)
    {
        return VacationRequests.Contains(curDay);
    }

    public bool IsWorking(DateTime curDay)
    {
        return WorkDays.Contains(curDay);
    }
}