using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly MedicalContext _context;

    public RegisterController(MedicalContext context)
    {
        _context = context;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetInviteInfo([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Token is required." });
        }

        Invitation? invitation = await _context.Invitations
            .FirstOrDefaultAsync(i =>
                i.Token == token && !i.Used && i.Expires > DateTime.UtcNow);

        if (invitation == null)
        {
            return NotFound(new
            { message = "Invitation not found or expired." });
        }

        Resident? resident = !string.IsNullOrEmpty(invitation.ResidentId)
            ? await _context.Residents.FirstOrDefaultAsync(r =>
                r.ResidentId == invitation.ResidentId)
            : null;

        return Ok(new RegisterInviteInfoResponse
        {
            hasEmailOnFile = resident != null,
            resident = resident == null
                ? null
                : new RegisterInviteInfoResponse.User
                {
                    firstName = resident.FirstName,
                    lastName = resident.LastName,
                    residentId = resident.ResidentId,
                    Email = resident.Email
                }
        });
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteRegistration(
        [FromBody] RegisterUpdateExistingUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return BadRequest(new
            { message = "Missing required fields." });
        }

        Invitation? invitation = await _context.Invitations
            .FirstOrDefaultAsync(i =>
                i.Token == request.Token && !i.Used &&
                i.Expires > DateTime.UtcNow);

        if (invitation == null)
        {
            return NotFound(new
            { message = "Invalid or expired invitation." });
        }

        if (string.IsNullOrEmpty(invitation.ResidentId))
        {
            return BadRequest(new
            {
                message
                    = "Resident ID not linked to invitation. Use register-new instead."
            });
        }

        Resident? resident =
            await _context.Residents.FirstOrDefaultAsync(r =>
                r.ResidentId == invitation.ResidentId);
        if (resident == null)
        {
            return NotFound(new { message = "Resident not found." });
        }

        resident.PhoneNum = request.Phone ?? resident.PhoneNum;

        if (request.Password != null)
        {
            resident.Password = HashPassword(request.Password);
        }
        invitation.Used = true;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration complete." });
    }

    [HttpPost("new")]
    public async Task<IActionResult> RegisterNewResident(
        [FromBody] RegisterCreateNewUserRequest request)
    {
        Invitation? invitation = await _context.Invitations
            .FirstOrDefaultAsync(i =>
                i.Token == request.Token && !i.Used &&
                i.Expires > DateTime.UtcNow);

        if (invitation == null)
        {
            return NotFound(new
            { message = "Invalid or expired invitation." });
        }

        Resident? existingResident
            = await _context.Residents.FirstOrDefaultAsync(r =>
                r.Email == request.Email);
        if (existingResident != null)
        {
            return BadRequest(new
            { message = "A resident with this email already exists." });
        }

        Resident newResident = new()
        {
            ResidentId = request.ResidentId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNum = request.Phone,
            Password = HashPassword(request.Password),
            GraduateYr = 1,
            WeeklyHours = 0,
            TotalHours = 0,
            BiYearlyHours = 0
        };

        _context.Residents.Add(newResident);
        invitation.Used = true;

        await _context.SaveChangesAsync();

        return Ok(
            new { message = "New resident registered successfully." });
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}