namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestValidateResponse : GenericResponse
{
    public ViolationResultResponse? RequesterViolationResults { get; set; }
    public ViolationResultResponse? RequesteeViolationResults { get; set; }
}