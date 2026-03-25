namespace MedicalDemo.Models.DTO.Responses;

public class GenericResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public static GenericResponse Failure(string message)
    {
        return new GenericResponse
        {
            Success = false,
            Message = message
        };
    }

    public static GenericResponse Successful(string message)
    {
        return new GenericResponse
        {
            Success = true,
            Message = message
        };
    }
}