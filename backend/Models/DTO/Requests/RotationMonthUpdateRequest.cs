using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class RotationMonthUpdateRequest
{
    [Required]
    public required Guid RotationTypeId { get; set; }
}