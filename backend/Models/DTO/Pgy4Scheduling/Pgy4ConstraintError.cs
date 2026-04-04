using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Pgy4Scheduling;

public class Pgy4ConstraintError
{
    public string Message { get; set; } = null!;
    public AlgorithmResident? Resident { get; set; }
    public MonthOfYear? CalendarMonthIndex { get; set; }
}