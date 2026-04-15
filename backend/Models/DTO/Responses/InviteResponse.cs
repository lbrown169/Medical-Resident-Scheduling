namespace MedicalDemo.Models.DTO.Responses;

public class InviteResponse
{
    public string? ResidentId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public DateTimeOffset Expires { get; set; }
}