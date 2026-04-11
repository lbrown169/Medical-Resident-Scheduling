namespace MedicalDemo.Models.DTO.Responses;

public class DateValidationResponse : GenericResponse
{
    private DateValidationResponse() { }

    public ViolationResultResponse? ViolationResultResponse { get; set; }

    public static DateValidationResponse NonViolationFailure(string message)
    {
        return new DateValidationResponse
        {
            Success = false,
            Message = message,
            ViolationResultResponse = null
        };
    }

    public static DateValidationResponse ViolationFailure(ViolationResultResponse result, string message)
    {
        return new DateValidationResponse
        {
            Success = false,
            Message = message,
            ViolationResultResponse = result
        };
    }

    public static DateValidationResponse NoViolations()
    {
        return new DateValidationResponse
        {
            Success = true,
            Message = "",
            ViolationResultResponse = null
        };
    }
}