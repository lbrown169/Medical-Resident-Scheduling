using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Responses;

public class ScheduleResponse
{
    public required Guid ScheduleId { get; set; }
    public required ScheduleStatusResponse Status { get; set; }
    public required int GeneratedYear { get; set; }

}