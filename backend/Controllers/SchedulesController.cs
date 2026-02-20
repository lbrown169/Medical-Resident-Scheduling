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

    public SchedulesController(ILogger<SchedulesController> logger, MedicalContext context, ScheduleConverter scheduleConverter)
    {
        _logger = logger;
        _context = context;
        _scheduleConverter = scheduleConverter;
    }

    // GET: api/schedules/
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleResponse>>> GetSchedules(
        [FromQuery] Guid? scheduleId = null,
        [FromQuery] int? generatedYear = null,
        [FromQuery] Semester? semester = null,
        [FromQuery] ScheduleStatus? status = null)
    {
        IQueryable<Schedule> query = _context.Schedules
            .Include(s => s.Dates)
            .AsQueryable();

        // filters
        if (scheduleId.HasValue)
        {
            query = query.Where(s => s.ScheduleId == scheduleId.Value);
        }

        if (generatedYear.HasValue)
        {
            query = query.Where(s => s.GeneratedYear == generatedYear.Value);
        }

        if (semester.HasValue)
        {
            query = query.Where(s => s.Semester == semester.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        List<Schedule> schedules = await query.ToListAsync();

        if (!schedules.Any())
        {
            return NotFound();
        }
        // dictionary of resident ID to hours for the Schedule
        List<ScheduleResponse> scheduleResponses = schedules
            .Select(schedule =>
            {
                ScheduleResponse response
                    = _scheduleConverter.CreateScheduleResponseFromSchedule(schedule);
                if (schedule.Dates.Any())
                {
                    response.ResidentHours = schedule.Dates
                        .GroupBy(d => d.ResidentId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Sum(d => d.Hours)
                        );
                    response.TotalHours = response.ResidentHours.Values.Sum();
                    response.TotalResidents = response.ResidentHours.Count();
                }
                return response;
            })
            .ToList();

        return Ok(scheduleResponses);
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