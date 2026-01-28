using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class InviteCreateRequest
{
    [Required]
    public required string Email { get; set; }
}