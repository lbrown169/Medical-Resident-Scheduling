namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4ResidenRotationScheduleResponse
{
    public ResidentResponse Resident { get; set; } = null!;

    public List<RotationResponse> Rotations { get; set; } = null!;
}