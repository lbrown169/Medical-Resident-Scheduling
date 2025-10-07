using MedicalDemo.Interfaces;
using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InviteController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly IEmailSendService _emailSendService;

    public InviteController(MedicalContext context, IEmailSendService emailSendService)
    {
        _context = context;
        _emailSendService = emailSendService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendInvitation(
        [FromBody] InviteRequest request)
    {
        //Check if the email exists already
        Residents? resident = await _context.residents
            .FirstOrDefaultAsync(r => r.email == request.Email);

        //Create token and expiration time
        string token = Guid.NewGuid().ToString();
        DateTime expires = DateTime.UtcNow.AddHours(24);

        //Save token to Invitations table
        Invitation invitation = new()
        {
            token = token,
            resident_id = resident?.resident_id,
            expires = expires,
            used = false
        };

        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync();

        //Build link based on whether resident was found or not
        string url;
        if (resident != null)
        {
            url = $"https://psycall.net/register?token={token}";
        }
        else
        {
            url
                = $"https://psycall.net/register-new?token={token}&email={Uri.EscapeDataString(request.Email)}";
        }


        //Send email
        bool success
            = await _emailSendService.SendInvitationEmailAsync(request.Email, url);

        if (!success)
        {
            return StatusCode(500,
                new { success = false, message = "Email sending failed." });
        }

        return Ok(new { success = true });
    }
}

public class InviteRequest
{
    public string Email { get; set; }
}