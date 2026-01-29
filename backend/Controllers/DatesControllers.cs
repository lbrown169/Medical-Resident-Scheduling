using MedicalDemo.Algorithm;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatesController : ControllerBase
{
    private readonly ILogger<DatesController> _logger;
    private readonly MedicalContext _context;
    private readonly DateConverter _dateConverter;

    public DatesController(MedicalContext context, DateConverter dateConverter, ILogger<DatesController> logger)
    {
        _context = context;
        _dateConverter = dateConverter;
        _logger = logger;
    }

    // POST: api/dates
    [HttpPost]
    public async Task<IActionResult> CreateDate([FromBody] DateCreateRequest request)
    {
        Date date = _dateConverter.CreateDateFromDateCreateRequest(request);
        Resident? resident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentId == request.ResidentId);

        if (resident == null)
        {
            return BadRequest();
        }

        date.Hours = CallShiftTypeExtensions
            .GetCallShiftTypeForDate(date.ShiftDate, resident.GraduateYr)
            .GetHours();

        _context.Dates.Add(date);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDates),
            new { id = date.DateId }, date);
    }

    // GET: api/dates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DateResponse>>> GetDates(
        [FromQuery] Guid? schedule_id,
        [FromQuery] string? resident_id,
        [FromQuery] DateOnly? date,
        [FromQuery] CallShiftType? call_type
    )
    {
        IQueryable<Date> query = _context.Dates.Include(d => d.Resident).AsQueryable();

        if (schedule_id is not null)
        {
            query = query.Where(d => d.ScheduleId == schedule_id);
        }

        if (!string.IsNullOrEmpty(resident_id))
        {
            query = query.Where(d => d.ResidentId == resident_id);
        }

        if (date is not null)
        {
            query = query.Where(d => d.ShiftDate == date.Value);
        }

        if (call_type is not null)
        {
            query = query.Where(d => d.CallType == call_type);
        }

        List<DateResponse> results = await query
            .Include(d => d.Resident)
            .Select(d => _dateConverter.CreateDateResponseFromDate(d))
            .ToListAsync();

        return Ok(results);
    }

    // GET: api/dates/published
    [HttpGet("published")]
    public async Task<ActionResult<IEnumerable<DateResponse>>>
        GetPublishedDates()
    {
        List<DateResponse> publishedDates = await _context.Dates
            .Include(d => d.Resident)
            .Where(d => d.Schedule.Status == ScheduleStatus.Published)
            .Select(d => _dateConverter.CreateDateResponseFromDate(d)).ToListAsync();

        return Ok(publishedDates);
    }

    // PUT: api/dates/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDate(Guid id,
        [FromBody] DateUpdateRequest updatedDate)
    {
        Date? existingDate = await _context.Dates.Include(d => d.Resident).FirstOrDefaultAsync(d => d.DateId == id);
        if (existingDate == null)
        {
            return NotFound();
        }

        // Update fields
        _dateConverter.UpdateDateFromDateUpdateRequest(existingDate, updatedDate);

        Resident? resident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentId == existingDate.ResidentId);
        if (resident == null)
        {
            return BadRequest();
        }

        if (updatedDate.Hours is null)
        {
            existingDate.Hours = CallShiftTypeExtensions
                .GetCallShiftTypeForDate(existingDate.ShiftDate,
                    resident.GraduateYr).GetHours();
        }

        try
        {
            await _context.SaveChangesAsync();
            await _context.Entry(existingDate).ReloadAsync();
            return Ok(_dateConverter.CreateDateResponseFromDate(existingDate)); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update the date");
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // DELETE: api/dates/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDate(Guid id)
    {
        Date? existingDate = await _context.Dates.FindAsync(id);
        if (existingDate == null)
        {
            return NotFound();
        }

        _context.Dates.Remove(existingDate);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
}