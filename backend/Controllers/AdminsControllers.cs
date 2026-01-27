using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminsController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly AdminConverter _adminConverter;

    public AdminsController(MedicalContext context, AdminConverter adminConverter)
    {
        _context = context;
        _adminConverter = adminConverter;
    }

    // GET: api/Admins
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminResponse>>> GetAdmins()
    {
        return await _context.Admins.Select(a => _adminConverter.CreateAdminResponseFromAdmin(a))
            .ToListAsync();
    }

    // PUT: api/Admins/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdmin(string id, AdminUpdateRequest adminUpdateRequest)
    {
        Admin? admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }

        _adminConverter.UpdateAdminFromAdminUpdateRequest(admin, adminUpdateRequest);
        int affected = await _context.SaveChangesAsync();

        return affected > 0 ? Ok(_adminConverter.CreateAdminResponseFromAdmin(admin)) : NoContent();
    }

    // POST: api/Admins/promote-resident/{residentId}
    [HttpPost("promote-resident/{residentId}")]
    public async Task<IActionResult> PromoteResidentToAdmin(string residentId)
    {
        Resident? resident
            = await _context.Residents.FindAsync(residentId);
        if (resident == null)
        {
            return NotFound("Resident not found.");
        }

        // Check if admin already exists with this ID
        Admin? existingAdmin
            = await _context.Admins.FindAsync(residentId);
        if (existingAdmin != null)
        {
            return BadRequest("Admin already exists with this ID.");
        }

        // Create new admin account
        Admin newAdmin = _adminConverter.CreateAdminFromResident(resident);

        // Add admin and remove resident
        _context.Admins.Add(newAdmin);
        _context.Residents.Remove(resident);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_adminConverter.CreateAdminResponseFromAdmin(newAdmin));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while promoting resident: {ex.Message}");
        }
    }

    // DELETE: api/Admins/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdmin(string id)
    {
        Admin? admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}