using MedicalDemo.Models.DTO.Responses;

namespace MedicalDemo.Models;

public class RequesteeValidationResult : GenericResponse
{
    public ViolationResultResponse? ViolationResults { get; set; }
}