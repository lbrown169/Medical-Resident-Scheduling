
namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4RotationScheduleOverrideListResponse
{
    public int Count { get; set; }

    public Pgy4RotationScheduleResponse Schedule { get; set; } = null!;

    public List<Pgy4RotationScheduleOverrideResponse> Overrides { get; set; } = [];
}