using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class SwapRequestUpdateRequest
{
    public string? RequesterId { get; set; }
    public string? RequesteeId { get; set; }
    public DateOnly? RequesterDate { get; set; }
    public DateOnly? RequesteeDate { get; set; }
    public string? Details { get; set; }
}