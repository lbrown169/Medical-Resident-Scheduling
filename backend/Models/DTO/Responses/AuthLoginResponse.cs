// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Responses;

public class AuthLoginResponse
{
    public required bool success { get; set; }
    public string? message { get; set; }
    public string? token { get; set; }
    public string? userType { get; set; }
    public User? resident { get; set; }
    public User? admin { get; set; }

    public class User
    {
        public required string id { get; set; }
        public required string email { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string phoneNum { get; set; }
    }
}