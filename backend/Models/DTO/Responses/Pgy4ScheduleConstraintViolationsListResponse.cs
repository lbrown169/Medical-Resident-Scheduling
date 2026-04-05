namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4ScheduleConstraintViolationsListResponse
{
    public Pgy4RotationScheduleResponse Schedule { get; set; } = null!;

    public List<Pgy4ScheduleConstraintViolationResponse> Violations { get; set; } = null!;
}