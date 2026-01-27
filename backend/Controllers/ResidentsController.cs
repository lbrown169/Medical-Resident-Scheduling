using MedicalDemo.Models;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Adjust namespace based on your project

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResidentsController : ControllerBase
{
    private readonly MedicalContext _context;

    public ResidentsController(MedicalContext context)
    {
        _context = context;
    }

    // GET: api/Residents
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Resident>>> GetResidents()
    {
        List<Resident> residents = await _context.Residents.AsNoTracking().ToListAsync();
        Dictionary<string, int> hoursMapping = (await _context.Dates
                .ToListAsync())
            .GroupBy(d => d.ResidentId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(d => d.Hours)
            );

        foreach (Resident resident in residents)
        {
            if (hoursMapping.TryGetValue(resident.ResidentId, out int hours))
            {
                resident.TotalHours = hours;
            }
            else
            {
                resident.TotalHours = 0;
            }
        }

        return Ok(residents);
    }

    // GET: api/residents/filter?resident_id=&first_name=&last_name=&graduate_yr=2&email=&password=&phone_num=&weekly_hours=&total_hours=&bi_yearly_hours
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Resident>>> FilterResidents(
        [FromQuery] string? resident_id,
        [FromQuery] string? first_name,
        [FromQuery] string? last_name,
        [FromQuery] int? graduate_yr,
        [FromQuery] string? email,
        [FromQuery] string? password,
        [FromQuery] string? phone_num,
        [FromQuery] int? weekly_hours,
        [FromQuery] int? total_hours,
        [FromQuery] int? bi_yearly_hours)
    {
        IQueryable<Resident> query = _context.Residents.AsQueryable();

        if (!string.IsNullOrEmpty(resident_id))
        {
            query = query.Where(r => r.ResidentId == resident_id);
        }

        if (!string.IsNullOrEmpty(first_name))
        {
            query = query.Where(r => r.FirstName.Contains(first_name));
        }

        if (!string.IsNullOrEmpty(last_name))
        {
            query = query.Where(r => r.LastName.Contains(last_name));
        }

        if (graduate_yr is not null)
        {
            query = query.Where(r => r.GraduateYr == graduate_yr);
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(r => r.Email.Contains(email));
        }

        if (!string.IsNullOrEmpty(password))
        {
            query = query.Where(r =>
                r.Password ==
                password); // Consider not exposing password filters
        }

        if (!string.IsNullOrEmpty(phone_num))
        {
            query = query.Where(r => r.PhoneNum.Contains(phone_num));
        }

        if (weekly_hours is not null)
        {
            query = query.Where(r => r.WeeklyHours == weekly_hours);
        }

        if (total_hours is not null)
        {
            query = query.Where(r => r.TotalHours == total_hours);
        }

        if (bi_yearly_hours is not null)
        {
            query = query.Where(r => r.BiYearlyHours == bi_yearly_hours);
        }

        List<Resident> results = await query.ToListAsync();

        if (!results.Any())
        {
            return NotFound("No residents matched the filter criteria.");
        }

        return Ok(results);
    }


    // PUT: api/residents/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResident(string id,
        [FromBody] Resident updatedResident)
    {
        if (id != updatedResident.ResidentId)
        {
            return BadRequest("Resident ID in URL and body do not match.");
        }

        Resident? existingResident
            = await _context.Residents.FindAsync(id);
        if (existingResident == null)
        {
            return NotFound("Resident not found.");
        }

        // Update fields
        existingResident.FirstName = updatedResident.FirstName;
        existingResident.LastName = updatedResident.LastName;
        existingResident.GraduateYr = updatedResident.GraduateYr;
        existingResident.Email = updatedResident.Email;
        existingResident.Password = updatedResident.Password;
        existingResident.PhoneNum = updatedResident.PhoneNum;
        existingResident.WeeklyHours = updatedResident.WeeklyHours;
        existingResident.TotalHours = updatedResident.TotalHours;
        existingResident.BiYearlyHours = updatedResident.BiYearlyHours;
        existingResident.HospitalRoleProfile = updatedResident.HospitalRoleProfile;

        try
        {
            await _context.SaveChangesAsync();
            return
                Ok(
                    existingResident); // returns the updated resident object
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the resident: {ex.Message}");
        }
    }

    // POST: api/Residents/demote-admin/{adminId}
    [HttpPost("demote-admin/{adminId}")]
    public async Task<IActionResult> DemoteAdminToResident(string adminId)
    {
        Admin? admin = await _context.Admins.FindAsync(adminId);
        if (admin == null)
        {
            return NotFound("Admin not found.");
        }

        // Check if resident already exists with this ID
        Resident? existingResident
            = await _context.Residents.FindAsync(adminId);
        if (existingResident != null)
        {
            return BadRequest("Resident already exists with this ID.");
        }

        // Create new resident account with default values
        Resident newResident = new()
        {
            ResidentId = admin.AdminId,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Email = admin.Email,
            Password = admin.Password,
            PhoneNum = admin.PhoneNum,
            GraduateYr = 1, // Default PGY level
            WeeklyHours = 0,
            TotalHours = 0,
            BiYearlyHours = 0
        };

        // Add resident and remove admin
        _context.Residents.Add(newResident);
        _context.Admins.Remove(admin);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(newResident);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while demoting admin: {ex.Message}");
        }
    }

    // POST: api/Residents
    [HttpPost]
    public async Task<ActionResult<Resident>> PostResident(Resident resident)
    {
        _context.Residents.Add(resident);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetResident",
            new { id = resident.ResidentId }, resident);
    }

    // DELETE: api/Residents/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResident(string id)
    {
        Resident? resident = await _context.Residents.FindAsync(id);
        if (resident == null)
        {
            return NotFound();
        }

        _context.Residents.Remove(resident);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}