using MedicalDemo.Interfaces;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Models.DTO.Requests
{
}

namespace MedicalDemo.Controllers
{
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
            [FromBody] InviteCreateRequest createRequest)
        {
            //Check if the email exists already
            Resident? resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.Email == createRequest.Email);

            //Create token and expiration time
            string token = Guid.NewGuid().ToString();
            DateTime expires = DateTime.UtcNow.AddHours(24);

            //Save token to Invitations table
            Invitation invitation = new()
            {
                Token = token,
                ResidentId = resident?.ResidentId,
                Expires = expires,
                Used = false
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
                    = $"https://psycall.net/register-new?token={token}&email={Uri.EscapeDataString(createRequest.Email)}";
            }


            //Send email
            bool success
                = await _emailSendService.SendInvitationEmailAsync(createRequest.Email, url);

            if (!success)
            {
                return StatusCode(500, new  InviteResponse
                {
                    success = false,
                    message = "Email sending failed."
                });
            }

            return Ok(new InviteResponse
            {
                success = true
            });
        }
    }
}