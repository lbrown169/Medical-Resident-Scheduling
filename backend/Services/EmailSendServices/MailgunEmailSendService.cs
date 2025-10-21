using MedicalDemo.Interfaces;

namespace MedicalDemo.Services.EmailSendServices;

public class MailgunEmailSendService : IEmailSendService
{
    private readonly ILogger<MailgunEmailSendService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public MailgunEmailSendService(
        ILogger<MailgunEmailSendService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SendInvitationEmailAsync(string toEmail, string link)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MailgunEmailSendService));

        string? fromEmail = _configuration.GetValue<string>("Mailgun:FromEmail");
        if (fromEmail == null)
        {
            throw new Exception("Mailgun:FromEmail is not set");
        }

        FormUrlEncodedContent formData = new([
            new KeyValuePair<string, string>("from", $"Psycall <{fromEmail}>"),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", "Psycall Invitation to " +
                "Register"),
            new KeyValuePair<string, string>("text", $"Hello,\n\nAdmin would " +
                $"like you to create a Psycall account.\n\nPlease use the " +
                $"following link:\n{link}\n\nThank you,\nPsycall Admin"),
            new KeyValuePair<string, string>("html", $"""
                <h3>Hello!</h3>
                <p>Admin would like you to create a Psycall account.</p>
                <p>Please click this link to complete your registration:</p>
                <a href='{link}'>{link}</a>
                <p>Thank you,<br/>Psycall Admin</p>
            """)
        ]);

        HttpResponseMessage response = await httpClient.PostAsync((string?) null, formData);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogError("Failed to send invite email to {toEmail} ({link}). Data: {json}", toEmail, link, await response.Content.ReadAsStringAsync());
        return false;

    }
}