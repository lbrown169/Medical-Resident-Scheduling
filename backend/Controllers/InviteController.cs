using System.Diagnostics.CodeAnalysis;
using MedicalDemo.Converters;
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
    [SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
    public class InviteController : ControllerBase
    {
        private readonly MedicalContext _context;
        private readonly IEmailSendService _emailSendService;
        private readonly InvitationConverter _invitationConverter;

        public InviteController(MedicalContext context, IEmailSendService emailSendService, InvitationConverter invitationConverter)
        {
            _context = context;
            _emailSendService = emailSendService;
            _invitationConverter = invitationConverter;
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

            if (await _context.Invitations.AnyAsync(i => i.Email.ToLower() == createRequest.Email.ToLower()))
            {
                return BadRequest(
                    GenericResponse.Failure("An invitation for that email already exists"));
            }

            //Save token to Invitations table
            Invitation invitation = new()
            {
                Token = token,
                ResidentId = resident?.ResidentId,
                Email = createRequest.Email,
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
                return StatusCode(500, new GenericResponse
                {
                    Success = false,
                    Message = "Email sending failed."
                });
            }

            return Ok(new GenericResponse
            {
                Success = true
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<InviteResponse>>> GetAllInvitations()
        {
            List<Invitation> invitations = await _context.Invitations
                .ToListAsync();

            IEnumerable<string> residentIds
                = invitations.Where(i => i.ResidentId != null).Select(i => i.ResidentId!);

            Dictionary<string, Resident> residents = await _context.Residents
                .Where(r => residentIds.Contains(r.ResidentId))
                .ToDictionaryAsync(r => r.ResidentId);

            Resident? ResidentSelector(string? id) => id == null ? null : residents.GetValueOrDefault(id);

            return Ok(
                invitations.Select(
                    invitation => _invitationConverter.CreateInvitationResponseFromModel(invitation, ResidentSelector(invitation.ResidentId))
                )
            );
        }

        [HttpDelete("{email}")]
        public async Task<ActionResult> DeleteInvitation([FromRoute] string email)
        {
            Invitation? invitation = await _context.Invitations.FirstOrDefaultAsync(i => i.Email.ToLower() == email.ToLower());
            if (invitation == null)
            {
                return BadRequest(GenericResponse.Failure("No invitation with that email exists"));
            }

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}