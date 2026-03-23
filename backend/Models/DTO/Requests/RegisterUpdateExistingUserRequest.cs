using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class RegisterUpdateExistingUserRequest
{
    [Required]
    public required string Token { get; set; }
    public string? Phone { get; set; }
    public string? Password { get; set; }
}