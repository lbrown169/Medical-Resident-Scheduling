using MedicalDemo.Algorithm;
using MedicalDemo.Extensions;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Models.DTO.Responses;

public class DateCallTypeShiftResponse
{
    public int Id { get; set; }
    public string Description { get; set; }

    public DateCallTypeShiftResponse(CallShiftType callShiftType)
    {
        Id = (int)callShiftType;
        Description = callShiftType.GetDisplayName();
    }
}