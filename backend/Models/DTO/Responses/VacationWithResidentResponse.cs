namespace MedicalDemo.Models.DTO.Responses;

public class VacationWithResidentResponse
{
    public Guid VacationId { get; set; }
    public required string ResidentId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly Date { get; set; }
    public required string Reason { get; set; }
    public required string Status { get; set; }
    public required string? Details { get; set; }
    public required string GroupId { get; set; } = string.Empty;
    public required string? HalfDay { get; set; }
}