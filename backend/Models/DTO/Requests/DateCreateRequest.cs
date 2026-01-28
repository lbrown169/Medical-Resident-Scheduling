using System.ComponentModel.DataAnnotations;
using MedicalDemo.Algorithm;

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

}