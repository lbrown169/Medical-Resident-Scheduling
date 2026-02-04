// ReSharper disable InconsistentNaming
namespace MedicalDemo.Models.DTO.Responses;

public class RegisterInviteInfoResponse
{
    public bool hasEmailOnFile { get; set; }
    public User? resident { get; set; }

    public class User
    {
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string residentId { get; set; }
        public required string Email { get; set; }
    }
}