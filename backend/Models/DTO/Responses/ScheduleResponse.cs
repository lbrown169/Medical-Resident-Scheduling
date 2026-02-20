using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Responses;

public class ScheduleResponse
{
    public required Guid ScheduleId { get; set; }
    public required ScheduleStatusResponse Status { get; set; }
    public required int GeneratedYear { get; set; }
    public required SemesterInfo Semester { get; set; }
    public Dictionary<string, int> ResidentHours { get; set; } = new();
    public int TotalHours { get; set; }
    public int TotalResidents { get; set; }
}

public class SemesterInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}