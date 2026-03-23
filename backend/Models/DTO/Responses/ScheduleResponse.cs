using MedicalDemo.Enums;
using MedicalDemo.Extensions;
namespace MedicalDemo.Models.DTO.Responses;

public class ScheduleResponse
{
    public required Guid ScheduleId { get; set; }
    public required ScheduleStatusResponse Status { get; set; }
    public required int Year { get; set; }
    public required SemesterInfoResponse Semester { get; set; }
    public Dictionary<string, int> ResidentHours { get; set; } = new();
    public int TotalHours { get; set; }
    public int TotalResidents { get; set; }
}