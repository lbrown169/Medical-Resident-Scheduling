using MedicalDemo.Algorithm;

namespace MedicalDemo.Models.DTO.Scheduling;

public class DatesDTO
{
    public Guid DateId { get; set; }
    public Guid ScheduleId { get; set; }
    public string ResidentId { get; set; }
    public DateOnly Date { get; set; }
    public CallShiftType CallType { get; set; }
    public int Hours { get; set; }
    public bool IsCommitted { get; set; }
}