using System.ComponentModel.DataAnnotations;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Models.DTO.Requests;

public class DateCreateRequest
{
    [Required]
    public required Guid ScheduleId { get; set; }

    [Required]
    public required string ResidentId { get; set; }

    [Required]
    public required DateOnly ShiftDate { get; set; }

    [Required]
    public required CallShiftType CallType { get; set; }

    public int? Hours { get; set; }
}