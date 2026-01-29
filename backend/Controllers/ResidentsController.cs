using MedicalDemo.Converters;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Adjust namespace based on your project

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResidentsController : ControllerBase
{
    private readonly ILogger<ResidentsController> _logger;
    private readonly MedicalContext _context;
    private readonly ResidentConverter _residentConverter;

    public ResidentsController(MedicalContext context, ResidentConverter residentConverter, ILogger<ResidentsController> logger)
    {
        _context = context;
        _residentConverter = residentConverter;
        _logger = logger;
    }

    // GET: api/residents/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<ResidentResponse>>> GetResident(Guid id)
    {
        Resident? resident = await _context.Residents.FindAsync(id);

        if (resident == null)
        {
            return NotFound();
        }

        return Ok(_residentConverter.CreateResidentResponseFromResident(resident));
    }

    // GET: api/residents?first_name=&last_name=&graduate_yr=2&email=&password=&phone_num=&weekly_hours=&total_hours=&bi_yearly_hours
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResidentResponse>>> GetResidents(
        [FromQuery] string? first_name,
        [FromQuery] string? last_name,
        [FromQuery] int? graduate_yr,
        [FromQuery] string? email,
        [FromQuery] string? phone_num,
        [FromQuery] int? weekly_hours,
        [FromQuery] int? total_hours,
        [FromQuery] int? bi_yearly_hours)
    {
        List<Date> dates = await _context.Dates.ToListAsync();

        Dictionary<string, int> totalHoursMapping = dates
            .GroupBy(d => d.ResidentId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(d => d.Hours)
            );

        DateOnly sunday = DateOnly.FromDateTime(DateTime.Now).AddDays(-(int)DateTime.Today.DayOfWeek);
        DateOnly saturday = DateOnly.FromDateTime(DateTime.Now).AddDays(6-(int)DateTime.Today.DayOfWeek);
        Dictionary<string, int> weekHoursMapping = dates
            .Where(d => d.ShiftDate >= sunday && d.ShiftDate <= saturday)
            .GroupBy(d => d.ResidentId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(d => d.Hours)
            );

        IQueryable<Resident> query = _context.Residents.AsQueryable();

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

        if (!string.IsNullOrEmpty(phone_num))
        {
            query = query.Where(r => r.PhoneNum.Contains(phone_num));
        }

        List<Resident> results = await query.ToListAsync();

        foreach (Resident resident in results)
        {
            resident.TotalHours = totalHoursMapping.GetValueOrDefault(resident.ResidentId, 0);
            resident.WeeklyHours = weekHoursMapping.GetValueOrDefault(resident.ResidentId, 0);
        }

        IEnumerable<Resident> residents = results;
        if (weekly_hours is not null)
        {
            residents = residents.Where(r => r.WeeklyHours == weekly_hours);
        }

        if (total_hours is not null)
        {
            residents = residents.Where(r => r.TotalHours == total_hours);
        }

        if (bi_yearly_hours is not null)
        {
            residents = residents.Where(r => r.BiYearlyHours == bi_yearly_hours);
        }

        return Ok(residents.Select(r => _residentConverter.CreateResidentResponseFromResident(r)));
    }


    // PUT: api/residents/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResident(string id,
        [FromBody] ResidentUpdateRequest updatedResident)
    {
        Resident? existingResident
            = await _context.Residents.FindAsync(id);
        if (existingResident == null)
        {
            return NotFound();
        }

        // Update fields
        _residentConverter.UpdateResidentWithResidentUpdateRequest(existingResident, updatedResident);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_residentConverter.CreateResidentResponseFromResident(existingResident));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update resident");
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
            _logger.LogError(ex, "Failed to demote admin");
            return StatusCode(500,
                $"An error occurred while demoting admin: {ex.Message}");
        }
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