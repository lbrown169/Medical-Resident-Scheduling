using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminsController : ControllerBase
{
    private readonly MedicalContext _context;

    public AdminsController(MedicalContext context)
    {
        _context = context;
    }

    // GET: api/Admins
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
    {
        return await _context.Admins.ToListAsync();
    }

    // GET: api/Admins/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Admin>> GetAdmin(string id)
    {
        Admin? admin = await _context.Admins.FindAsync(id);

        if (admin == null)
        {
            return NotFound();
        }

        return admin;
    }

    // POST: api/Admins
    [HttpPost]
    public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
    {
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAdmin", new { id = admin.AdminId },
            admin);
    }

    // PUT: api/Admins/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdmin(string id, Admin admin)
    {
        if (id != admin.AdminId)
        {
            return BadRequest();
        }

        _context.Entry(admin).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdminExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
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
        Admin newAdmin = new()
        {
            AdminId = resident.ResidentId,
            FirstName = resident.FirstName,
            LastName = resident.LastName,
            Email = resident.Email,
            Password = resident.Password,
            PhoneNum = resident.PhoneNum
        };

        // Add admin and remove resident
        _context.Admins.Add(newAdmin);
        _context.Residents.Remove(resident);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(newAdmin);
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

    private bool AdminExists(string id)
    {
        return _context.Admins.Any(e => e.AdminId == id);
    }
}