using MedicalDemo.Models.Entities;

namespace MedicalDemo.Models.DTO.Requests;

public class AdminUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNum { get; set; }
}