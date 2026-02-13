namespace MedicalDemo.Models.DTO.Responses;

public class PGY4ResidenRotationScheduleResponse
{
    public ResidentResponse Resident { get; set; } = null!;

    public List<RotationResponse> Rotations { get; set; } = null!;
}

public class PGY4RotationScheduleResponse
{
    public Guid ScheduleId { get; set; }

    public int ResidentCount { get; set; }

    public List<PGY4ResidenRotationScheduleResponse> Schedule { get; set; } = null!;
}