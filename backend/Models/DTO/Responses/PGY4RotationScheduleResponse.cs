namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4RotationScheduleResponse
{
    public Guid Pgy4RotationScheduleId { get; set; }

    public int ResidentCount { get; set; }

    public int Seed { get; set; }

    public int Year { get; set; }

    public bool IsPublished { get; set; }

    public List<Pgy4ResidenRotationScheduleResponse> Schedule { get; set; } = null!;
}