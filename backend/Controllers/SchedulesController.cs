//testing
using System.Text.Json;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
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
    private readonly RuleViolationService _ruleViolationService;

    public SchedulesController(ILogger<SchedulesController> logger, MedicalContext context, ScheduleConverter scheduleConverter, RuleViolationService ruleViolationService)
    {
        _logger = logger;
        _context = context;
        _scheduleConverter = scheduleConverter;
        _ruleViolationService = ruleViolationService;
    }

    // GET: api/schedules/
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleResponse>>> GetSchedules(
        [FromQuery] Guid? scheduleId = null,
        [FromQuery] int? year = null,
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

        if (year.HasValue)
        {
            query = query.Where(s => s.Year == year.Value);
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

        // dictionary of resident ID to hours for the Schedule
        List<ScheduleResponse> scheduleResponses = schedules
            .Select(schedule => _scheduleConverter.CreateScheduleResponseFromSchedule(schedule))
            .ToList();

        return Ok(scheduleResponses);
    }

    // GET: api/schedules/{id}/check
    [HttpGet("{id}/checkViolations")]
    public async Task<IActionResult> CheckScheduleViolations(Guid id,
        [FromQuery] string residentId,
        [FromQuery] DateOnly date,
        [FromQuery] bool adminOverride)
    {
        Schedule? existingSchedule = await _context.Schedules.FindAsync(id);
        if (existingSchedule == null)
        {
            return NotFound();
        }

        if (string.IsNullOrEmpty(residentId))
        {
            return BadRequest();
        }

        ViolationResult violationResult = await _ruleViolationService.EvaluateConstraints(id, residentId, date, false);
        ViolationResultResponse response = new ViolationResultResponse(violationResult);

        // if violation is overridable but have no adminOverride priviledges -> not allowed
        if (violationResult.IsViolation)
        {
            if (violationResult.IsOverridable == true && adminOverride)
            {
                response.IsAllowed = true;
            }
            else
            {
                response.IsAllowed = false;
            }
        }
        return Ok(response);
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

        bool updated = false;

        if (updateSchedule.Status != null)
        {
            // Only one schedule per year/semester can be published
            if (updateSchedule.Status == ScheduleStatus.Published)
            {
                bool isSchedulePublished = await _context.Schedules.AnyAsync(s =>
                    s.Year == existingSchedule.Year
                    && s.Semester == existingSchedule.Semester
                    && s.Status == ScheduleStatus.Published
                );
                if (isSchedulePublished)
                {
                    return Conflict(new GenericResponse
                    {
                        Success = false,
                        Message = $"A schedule for {existingSchedule.Semester.GetDisplayName()} {existingSchedule.Year} is already published"
                    });
                }
            }

            updated = existingSchedule.Status != updateSchedule.Status;
            existingSchedule.Status = (ScheduleStatus)updateSchedule.Status;
        }

        if (!updated)
        {
            return NoContent();
        }

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