using MedicalDemo.Algorithm;

namespace MedicalDemo.Models.DTO.Responses;

public class DateResponse
{
    public Guid DateId { get; set; }
    public Guid ScheduleId { get; set; }
    public required string ResidentId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly ShiftDate { get; set; }
    public required DateCallTypeShiftResponse CallType { get; set; }
    public int Hours { get; set; }
}