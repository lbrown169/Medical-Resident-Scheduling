namespace MedicalDemo.Models.DTO.Requests;

public class VacationUpdateRequest
{
    public DateOnly? Date { get; set; }
    public string? Reason { get; set; }
    public string? Details { get; set; }
    public string? HalfDay { get; set; }
}