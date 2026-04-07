namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestValidateResponse : GenericResponse
{
    public List<ViolationResultResponse>? violationResults { get; set; }
}