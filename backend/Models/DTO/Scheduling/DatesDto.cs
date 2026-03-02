using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Scheduling;

public class DatesDto
{
    public Guid DateId { get; set; }
    public Guid ScheduleId { get; set; }
    public string ResidentId { get; set; }
    public DateOnly Date { get; set; }
    public CallShiftType CallType { get; set; }
    public int Hours { get; set; }
    public bool IsCommitted { get; set; }
}