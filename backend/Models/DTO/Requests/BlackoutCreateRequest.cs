using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class BlackoutCreateRequest
{
    [Required]
    public required string ResidentId { get; set; }
    [Required]
    public required DateOnly Date { get; set; }
}