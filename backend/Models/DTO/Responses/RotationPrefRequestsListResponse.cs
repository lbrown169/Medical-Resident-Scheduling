namespace MedicalDemo.Models.DTO.Responses;

public class RotationPrefRequestsListResponse
{
    public int Count { get; set; }

    public List<RotationPrefResponse> RotationPrefRequests { get; set; } = [];
}