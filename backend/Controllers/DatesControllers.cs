using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
    private readonly RuleViolationService _ruleViolationService;

    public DatesController(
        MedicalContext context,
        DateConverter dateConverter,
        ILogger<DatesController> logger,
        RuleViolationService ruleViolationService
    )
    {
        _context = context;
        _dateConverter = dateConverter;
        _logger = logger;
        _ruleViolationService = ruleViolationService;
    }

    // POST: api/dates
    [HttpPost]
    public async Task<IActionResult> CreateDate([FromBody] DateCreateRequest request)
    {
        Date date = _dateConverter.CreateDateFromDateCreateRequest(request);
        Resident? resident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentId == request.ResidentId);

        if (resident?.GraduateYr is null)
        {
            return BadRequest();
        }

        if (request.CallType is not CallShiftType.Custom)
        {
            if (CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date.ShiftDate, resident.GraduateYr.Value) is
                not { } shiftType)
            {
                return BadRequest(new GenericResponse
                {
                    Success = false,
                    Message = "Shift is not valid for given resident year"
                });
            }

            date.Hours = request.Hours ?? shiftType.GetHours();
        }
        else
        {
            if (request.Hours is null)
            {
                return BadRequest(new GenericResponse
                {
                    Success = false,
                    Message = "Hours is required if the CallType is Custom"
                });
            }

            date.Hours = request.Hours.Value;
        }

        _context.Dates.Add(date);
        await _context.SaveChangesAsync();

        return Created();
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

    // GET: api/dates/call-types
    [HttpGet("call-types")]
    public async Task<ActionResult<DateCallTypeShiftListResponse>>
        GetCallShiftType(
            [FromQuery] Guid schedule_id,
            [FromQuery] string resident_id,
            [FromQuery] DateOnly date)
    {
        (bool checkPassed, string? checkError, Resident? resident) = await _ruleViolationService.CheckResidentScheduledOnDate(schedule_id, resident_id, date);
        if (!checkPassed || resident?.GraduateYr == null)
        {
            return BadRequest(new GenericResponse()
            {
                Success = false,
                Message = checkError ?? "Failed to check if resident was scheduled on date"
            });
        }

        // calc gradYr accounting for PGYear offset, if any
        int graduateYr = resident.GetGraduateYrForDate(date);

        // returns valid call type given year and date, if null returns custom shift
        CallShiftType resultCallType =
            CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(date, graduateYr) ?? CallShiftType.Custom;

        List<DateCallTypeShiftResponse> resultCallTypes = [new(resultCallType)];

        if (resultCallType != CallShiftType.Custom)
        {
            resultCallTypes.Add(new DateCallTypeShiftResponse(CallShiftType.Custom));
        }

        return Ok(resultCallTypes);
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
        [FromQuery] DateUpdateRequest updatedDate,
        [FromQuery] bool adminOverride)
    {
        Date? existingDate = await _context.Dates.Include(d => d.Resident).FirstOrDefaultAsync(d => d.DateId == id);
        if (existingDate == null)
        {
            return NotFound();
        }

        bool isDateOnlyUpdate = updatedDate.ShiftDate.HasValue && existingDate.ShiftDate != updatedDate.ShiftDate;

        // Update fields
        _dateConverter.UpdateDateFromDateUpdateRequest(existingDate, updatedDate);

        Resident? resident
            = await _context.Residents.FirstOrDefaultAsync(r =>
                r.ResidentId == existingDate.ResidentId);
        if (resident?.GraduateYr == null)
        {
            return BadRequest();
        }

        if (updatedDate.CallType is not null and not CallShiftType.Custom)
        {
            if (CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(existingDate.ShiftDate, resident.GraduateYr.Value) is
                not { } shiftType)
            {
                return BadRequest(new GenericResponse
                {
                    Success = false,
                    Message = "Shift is not valid for given resident year"
                });
            }

            existingDate.Hours = updatedDate.Hours ?? shiftType.GetHours();
        }
        else
        {
            if (updatedDate.Hours is not null)
            {
                existingDate.Hours = updatedDate.Hours.Value;
            }
        }

        // evaluate rule violations of update
        ViolationResult violationResult = await _ruleViolationService.EvaluateConstraints(existingDate.ScheduleId, resident.ResidentId, updatedDate.ShiftDate ?? existingDate.ShiftDate, isDateOnlyUpdate);
        ViolationResultResponse response = new(violationResult);

        if (violationResult.IsViolation)
        {
            if (violationResult.IsOverridable == true && !adminOverride)
            {
                return BadRequest(new
                {
                    Success = false,
                    message = "Unable to update the date: Constraint violations without adminOverride privileges.",
                    violationResultResponse = response
                });
            }

            if (violationResult.IsOverridable == false)
            {
                return BadRequest(new
                {
                    Success = false,
                    message = "Unable to update the date: Constraint violations cannot be overriden.",
                    violationResultResponse = response
                });
            }
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