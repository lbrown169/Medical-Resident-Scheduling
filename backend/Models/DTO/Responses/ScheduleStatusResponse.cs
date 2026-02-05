using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Models.DTO.Responses;

public class ScheduleStatusResponse
{
    public int Id { get; set; }
    public string Description { get; set; }

    public ScheduleStatusResponse(ScheduleStatus status)
    {
        Id = (int)status;
        Description = status.GetDisplayName();
    }
}