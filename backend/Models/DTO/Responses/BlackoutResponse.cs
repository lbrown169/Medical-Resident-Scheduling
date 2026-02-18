namespace MedicalDemo.Models.DTO.Responses;

public class BlackoutResponse
{
    public Guid BlackoutId { get; set; }
    public string ResidentId { get; set; } = null!;
    public DateOnly Date { get; set; }
}