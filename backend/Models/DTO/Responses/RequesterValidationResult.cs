using MedicalDemo.Models.DTO.Responses;

namespace MedicalDemo.Models;

public class RequesterValidationResult : GenericResponse
{
    public ViolationResultResponse? ViolationResults { get; set; }
}