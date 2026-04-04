using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4ScheduleConstraintErrorResponse
{
    public string Message { get; set; } = null!;
    public ResidentResponse? Resident { get; set; }
    public MonthOfYear? AcademicMonthIndex { get; set; }
}