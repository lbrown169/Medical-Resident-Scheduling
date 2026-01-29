using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class VacationCreateRequest
{
    [Required]
    public required string ResidentId { get; set; }
    [Required]
    public required DateOnly Date { get; set; }
    [Required]
    public required string Reason { get; set; }
    public string? Details { get; set; }
    [Required]
    public required string GroupId { get; set; }
    public string? HalfDay { get; set; }
}