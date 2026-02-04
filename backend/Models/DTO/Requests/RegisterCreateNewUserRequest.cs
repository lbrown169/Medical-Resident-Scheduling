using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class RegisterCreateNewUserRequest
{
    [Required]
    public required string Token { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string FirstName { get; set; }
    [Required]
    public required string LastName { get; set; }
    [Required]
    public required string ResidentId { get; set; }
    [Required]
    public required string Phone { get; set; }
    [Required]
    public required string Password { get; set; }
}