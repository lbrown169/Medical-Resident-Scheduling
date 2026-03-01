using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class Pgy4RotationScheduleOverrideRequest
{
    [Range(0, 11, ErrorMessage = "Academic month index must be between 0 and 11.")]
    public int AcademicMonthIndex { get; set; }

    public string ResidentId { get; set; } = null!;

    public Guid newRotationTypeId { get; set; }
}