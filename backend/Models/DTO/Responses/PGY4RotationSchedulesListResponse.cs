namespace MedicalDemo.Models.DTO.Responses;

public class PGY4RotationSchedulesListResponse
{
    public int Count { get; set; }

    public List<PGY4RotationScheduleResponse> Schedules { get; set; } = null!;
}