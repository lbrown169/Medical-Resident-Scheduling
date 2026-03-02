namespace MedicalDemo.Models.DTO.Responses;

public class Pgy4RotationSchedulesListResponse
{
    public int Count { get; set; }

    public List<Pgy4RotationScheduleResponse> Schedules { get; set; } = null!;
}