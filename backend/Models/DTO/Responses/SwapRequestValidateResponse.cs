namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestValidateResponse : GenericResponse
{
    public IndividualValidationResult? Requester { get; set; }
    public IndividualValidationResult? Requestee { get; set; }
}