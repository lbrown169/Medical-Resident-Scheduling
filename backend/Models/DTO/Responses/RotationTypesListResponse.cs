namespace MedicalDemo.Models.DTO.Responses;

public class RotationTypesListResponse
{
    public int Count { get; set; }

    public List<RotationTypeResponse> RotationTypes { get; set; } = [];
}