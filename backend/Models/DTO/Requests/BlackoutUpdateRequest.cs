using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class BlackoutUpdateRequest
{
    public string? ResidentId { get; set; }
    public DateOnly? Date { get; set; }
}