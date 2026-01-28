using MedicalDemo.Algorithm;

namespace MedicalDemo.Models.DTO.Requests;

public class DateUpdateRequest
{
    public Guid? ScheduleId { get; set; }
    public string? ResidentId { get; set; }
    public DateOnly? ShiftDate { get; set; }
    public CallShiftType? CallType { get; set; }
    public int? Hours { get; set; }
}