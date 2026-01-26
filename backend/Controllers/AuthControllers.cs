using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MedicalContext _context;

    public AuthController(MedicalContext context)
    {
        _context = context;
    }

    [HttpOptions]
    public IActionResult Options()
    {
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Resident resident)
    {
        bool exists = await _context.Residents
            .AnyAsync(r => r.Email == resident.Email);

        if (exists)
        {
            return Conflict(new
            { success = false, message = "Email already registered" });
        }

        string? hashedPassword
            = BCrypt.Net.BCrypt.HashPassword(resident.Password);

        Resident newResident = new()
        {
            ResidentId = resident.ResidentId,
            FirstName = resident.FirstName,
            LastName = resident.LastName,
            Email = resident.Email,
            Password = hashedPassword,
            PhoneNum = resident.PhoneNum,
            GraduateYr = resident.GraduateYr,
            WeeklyHours = 0,
            TotalHours = 0,
            BiYearlyHours = 0
        };

        _context.Residents.Add(newResident);
        await _context.SaveChangesAsync();

        return StatusCode(201, new { success = true });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            //check residents table first
            Resident? resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.Email == request.email);

            if (resident != null)
            {
                bool passwordMatch
                    = BCrypt.Net.BCrypt.Verify(request.password,
                        resident.Password);

                if (!passwordMatch)
                {
                    return Unauthorized(new
                    { success = false, message = "Invalid credentials" });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new
                {
                    success = true,
                    token,
                    userType = "resident",
                    resident = new
                    {
                        id = resident.ResidentId,
                        resident.Email,
                        firstName = resident.FirstName,
                        lastName = resident.LastName,
                        resident.PhoneNum
                    }
                });
            }

            //check admins table if not found in residents
            Admin? admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == request.email);

            if (admin != null)
            {
                bool passwordMatch
                    = BCrypt.Net.BCrypt.Verify(request.password,
                        admin.Password);

                if (!passwordMatch)
                {
                    return Unauthorized(new
                    { success = false, message = "Invalid credentials" });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new
                {
                    success = true,
                    token,
                    userType = "admin",
                    admin = new
                    {
                        id = admin.AdminId,
                        admin.Email,
                        firstName = admin.FirstName,
                        lastName = admin.LastName,
                        admin.PhoneNum
                    }
                });
            }

            //Not found in either
            return Unauthorized(new
            { success = false, message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new
                {
                    success = false,
                    message = $"Login error: {ex.Message}"
                });
        }
    }
}

public class LoginRequest
{
    public string email { get; set; }
    public string password { get; set; }
}