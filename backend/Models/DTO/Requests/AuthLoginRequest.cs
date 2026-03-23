// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class AuthLoginRequest
{
    [Required]
    public required string email { get; set; }
    [Required]
    public required string password { get; set; }
}