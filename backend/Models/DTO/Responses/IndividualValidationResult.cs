namespace MedicalDemo.Models.DTO.Responses;

public class IndividualValidationResult
{
    public required string Message { get; set; }
    public ViolationResultResponse? Violations { get; set; }
}