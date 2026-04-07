using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class SwapRequestConverter
{
    public SwapRequest CreateSwapRequestFromSwapRequestCreateRequest(
        SwapRequestCreateRequest createRequest)
    {
        return new SwapRequest
        {
            SwapRequestId = Guid.NewGuid(),
            RequesterId = createRequest.RequesterId,
            RequesteeId = createRequest.RequesteeId,
            RequesterDate = createRequest.RequesterDate,
            RequesteeDate = createRequest.RequesteeDate,
            Status = RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Details = createRequest.Details,
        };
    }

    public SwapRequestResponse CreateSwapRequestResponseFromSwapRequest(
        SwapRequest swapRequest)
    {
        return new SwapRequestResponse
        {
            SwapRequestId = swapRequest.SwapRequestId,
            ScheduleId = swapRequest.ScheduleId,
            RequesterId = swapRequest.RequesterId,
            RequesteeId = swapRequest.RequesteeId,
            RequesterDate = swapRequest.RequesterDate,
            RequesteeDate = swapRequest.RequesteeDate,
            Status = new SwapRequestStatusResponse(swapRequest.Status),
            CreatedAt = new DateTimeOffset(swapRequest.CreatedAt, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(swapRequest.UpdatedAt, TimeSpan.Zero),
            IsRead = swapRequest.IsRead,
            Details = swapRequest.Details,
        };
    }

    public void UpdateSwapRequestFromSwapRequestUpdates(SwapRequest swapRequest,
        UpdateSwapRequest updates)
    {
        swapRequest.IsRead = updates.IsRead ?? swapRequest.IsRead;
    }
}