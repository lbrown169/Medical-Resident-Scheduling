using MedicalDemo.Enums;

namespace MedicalDemo.Models.DTO.Requests;

public class UpdateScheduleRequest
{
    public ScheduleStatus? Status { get; set; }
}