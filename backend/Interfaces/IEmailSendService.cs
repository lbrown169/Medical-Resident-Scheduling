namespace MedicalDemo.Interfaces;

public interface IEmailSendService
{
    Task<bool> SendInvitationEmailAsync(string toEmail,
        string link);
}