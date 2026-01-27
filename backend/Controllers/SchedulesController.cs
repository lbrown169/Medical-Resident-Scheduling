using MedicalDemo.Enums;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Adjust namespace based on your project

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly MedicalContext _context;

    public SchedulesController(MedicalContext context)
    {
        _context = context;
    }

    // GET: api/schedules
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Schedule>>> GetAllSchedules()
    {
        List<Schedule> schedules = await _context.Schedules.ToListAsync();
        return Ok(schedules);
    }

    // GET: api/schedules/filter?status=
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Schedule>>> FilterSchedules(
        [FromQuery] ScheduleStatus status)
    {
        IQueryable<Schedule> query = _context.Schedules.AsQueryable();

        if (status != null)
        {
            query = query.Where(s => s.Status == status);
        }

        List<Schedule> results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No matching schedule records found.");
        }

        return Ok(results);
    }

    // GET: api/schedules/published-dates
    [HttpGet("published-dates")]
    public async Task<ActionResult<IEnumerable<ScheduleDatesDTO>>>
        GetPublishedDates()
    {
        List<ScheduleDatesDTO> publishedDates = await (
            from d in _context.Dates
            join s in _context.Schedules on d.ScheduleId equals s
                .ScheduleId
            where s.Status == ScheduleStatus.Published
            select new ScheduleDatesDTO
            {
                Date = d.ShiftDate,
                ResidentId = d.ResidentId,
                CallType = d.CallType
            }).ToListAsync();

        if (!publishedDates.Any())
        {
            return NotFound("No dates found for published schedules.");
        }

        return Ok(publishedDates);
    }

    // GET: api/schedules/under-review-dates
    [HttpGet("under-review-dates")]
    public async Task<ActionResult<IEnumerable<ScheduleDatesDTO>>>
        GetUnderReviewDates()
    {
        List<ScheduleDatesDTO> underReviewDates = await (
            from d in _context.Dates
            join s in _context.Schedules on d.ScheduleId equals s
                .ScheduleId
            where s.Status == ScheduleStatus.UnderReview
            select new ScheduleDatesDTO
            {
                Date = d.ShiftDate,
                ResidentId = d.ResidentId,
                CallType = d.CallType
            }).ToListAsync();

        if (!underReviewDates.Any())
        {
            return NotFound("No dates found for schedules under review.");
        }

        return Ok(underReviewDates);
    }


    // PUT: api/schedules/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id,
        [FromBody] UpdateScheduleDto updateSchedule)
    {
        Schedule? existingSchedule
            = await _context.Schedules.FindAsync(id);

        if (existingSchedule == null)
        {
            return NotFound("Schedule not found.");
        }

        if (existingSchedule.Status == updateSchedule.Status)
        {
            return NoContent();
        }

        // Only one schedule per year can be published
        if (updateSchedule.Status == ScheduleStatus.Published)
        {
            bool isSchedulePublished = await _context.Schedules.AnyAsync(s =>
                s.GeneratedYear == existingSchedule.GeneratedYear
                && s.Status == ScheduleStatus.Published
            );
            if (isSchedulePublished)
            {
                return Conflict(
                    $"A schedule for {existingSchedule.GeneratedYear} is already published");
            }
        }

        existingSchedule.Status = updateSchedule.Status;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingSchedule); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // DELETE: api/schedules/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        Schedule? schedule = await _context.Schedules.FindAsync(id);

        if (schedule == null)
        {
            return NotFound("Schedule not found.");
        }

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }
}