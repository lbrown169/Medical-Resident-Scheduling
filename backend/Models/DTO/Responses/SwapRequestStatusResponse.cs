using MedicalDemo.Enums;
using MedicalDemo.Extensions;

namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestStatusResponse
{
    public int Id { get; set; }
    public string Description { get; set; }

    public SwapRequestStatusResponse(RequestStatus requestStatus)
    {
        Id = (int)requestStatus;
        Description = requestStatus.GetDisplayName();
    }
}