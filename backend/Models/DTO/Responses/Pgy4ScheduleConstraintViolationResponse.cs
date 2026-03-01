namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4ScheduleConstraintViolationResponse
{
    public string ConstraintViolated { get; set; } = null!;

    public List<Pgy4ScheduleConstraintErrorResponse> Errors { get; set; } = null!;
}