using MedicalDemo.Algorithm;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Adjust namespace based on your project

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly ILogger<SchedulesController> _logger;
    private readonly MedicalContext _context;
    private readonly ScheduleConverter _scheduleConverter;
    private readonly DateConverter _dateConverter;

    public SchedulesController(ILogger<SchedulesController> logger, MedicalContext context, ScheduleConverter scheduleConverter, DateConverter dateConverter)
    {
        _logger = logger;
        _context = context;
        _scheduleConverter = scheduleConverter;
        _dateConverter = dateConverter;
    }

    // GET: api/schedules?status=&generatedYear=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleResponse>>> GetAllSchedules(
        [FromQuery] ScheduleStatus? status,
        [FromQuery] int? generatedYear)
    {
        IQueryable<Schedule> query = _context.Schedules.AsQueryable();

        if (status != null)
        {
            query = query.Where(s => s.Status == status);
        }

        if (generatedYear != null)
        {
            query = query.Where(s => s.GeneratedYear == generatedYear);
        }

        List<ScheduleResponse> results = await query.Select(s => _scheduleConverter.CreateScheduleResponseFromSchedule(s)).ToListAsync();

        return Ok(results);
    }

    // GET: api/schedules/published/{year}
    [HttpGet("published/{year}")]
    public async Task<ActionResult<ScheduleResponse>>
        GetPublishedSchedule(int year)
    {
        Schedule? schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Status == ScheduleStatus.Published && s.GeneratedYear == year);

        if (schedule == null)
        {
            return NotFound();
        }

        return Ok(_scheduleConverter.CreateScheduleResponseFromSchedule(schedule));
    }

    // PUT: api/schedules/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id,
        [FromBody] UpdateScheduleRequest updateSchedule)
    {
        Schedule? existingSchedule
            = await _context.Schedules.FindAsync(id);

        if (existingSchedule == null)
        {
            return NotFound();
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
                return Conflict(new GenericResponse
                {
                    Success = false,
                    Message = $"A schedule for {existingSchedule.GeneratedYear} is already published"
                });
            }
        }

        existingSchedule.Status = updateSchedule.Status;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_scheduleConverter.CreateScheduleResponseFromSchedule(existingSchedule)); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred updating the schedule {scheduleId}", id);
            return StatusCode(500, new GenericResponse
            {
                Success = false,
                Message = "An error occurred while updating the schedule"
            });
        }
    }

    // DELETE: api/schedules/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        Schedule? schedule = await _context.Schedules.FindAsync(id);

        if (schedule == null)
        {
            return NotFound();
        }

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }
}