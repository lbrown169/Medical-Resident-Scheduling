namespace MedicalDemo.Models.DTO.Responses;

public class UnsubmittedResidentsResponse
{
    public string Message { get; set; } = null!;

    public List<ResidentResponse> UnsubmittedResidents { get; set; } = [];
}