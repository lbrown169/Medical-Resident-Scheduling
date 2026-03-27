using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class RotationAssignRequest
{
    [Required]
    public required string ResidentId { get; set; }
}