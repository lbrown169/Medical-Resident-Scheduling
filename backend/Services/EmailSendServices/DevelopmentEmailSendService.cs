using MedicalDemo.Interfaces;

namespace MedicalDemo.Services.EmailSendServices;

public class DevelopmentEmailSendService : IEmailSendService
{
    private readonly ILogger<DevelopmentEmailSendService> _logger;

    public DevelopmentEmailSendService(
        ILogger<DevelopmentEmailSendService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendInvitationEmailAsync(string toEmail, string link)
    {
        _logger.LogInformation("An invite email to {toEmail} would be sent with link:\n{link}", toEmail, link);
        return Task.FromResult(true);
    }
}