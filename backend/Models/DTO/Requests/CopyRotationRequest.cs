using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class CopyRotationRequest
{
    [Required]
    public required int FromAcademicYear { get; set; }

    [Required]
    public required int ToAcademicYear { get; set; }
}