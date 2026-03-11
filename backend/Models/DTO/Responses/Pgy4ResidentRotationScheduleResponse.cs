namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4ResidentRotationScheduleResponse
{
    public ResidentResponse Resident { get; set; } = null!;

    public List<RotationResponse> Rotations { get; set; } = null!;
}