using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class RotationPrefSubmissionWindowRequest
{
    [Required]
    public DateTime AvailableDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}