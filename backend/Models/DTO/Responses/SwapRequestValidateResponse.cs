namespace MedicalDemo.Models.DTO.Responses;

public class SwapRequestValidateResponse : GenericResponse
{
    public RequesterValidationResult? Requester { get; set; }
    public RequesteeValidationResult? Requestee { get; set; }
}