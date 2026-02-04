using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
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
                    return Unauthorized(new AuthLoginResponse
                    {
                        success = false,
                        message = "Invalid credentials"
                    });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new AuthLoginResponse
                {
                    success = true,
                    token = token,
                    userType = "resident",
                    resident = new AuthLoginResponse.User
                    {
                        id = resident.ResidentId,
                        email = resident.Email,
                        firstName = resident.FirstName,
                        lastName = resident.LastName,
                        phoneNum = resident.PhoneNum
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
                    return Unauthorized(new AuthLoginResponse
                    {
                        success = false,
                        message = "Invalid credentials"
                    });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new AuthLoginResponse
                {
                    success = true,
                    token = token,
                    userType = "admin",
                    admin = new AuthLoginResponse.User
                    {
                        id = admin.AdminId,
                        email = admin.Email,
                        firstName = admin.FirstName,
                        lastName = admin.LastName,
                        phoneNum = admin.PhoneNum
                    }
                });
            }

            //Not found in either
            return Unauthorized(new AuthLoginResponse
            {
                success = false,
                message = "Invalid credentials"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthLoginResponse
            {
                success = false,
                message = $"Login error: {ex.Message}"
            });
        }
    }
}