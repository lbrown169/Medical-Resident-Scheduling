using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class SwapRequestCreateRequest
{
    [Required]
    public required string RequesterId { get; set; }
    [Required]
    public required string RequesteeId { get; set; }
    [Required]
    public required DateOnly RequesterDate { get; set; }
    [Required]
    public required DateOnly RequesteeDate { get; set; }
    public string? Details { get; set; }
}