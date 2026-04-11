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
            return BadRequest(
                DateValidationResponse.NonViolationFailure("Resident with active PGY status not found"));
        }

        if (request.Hours is null && request.CallType == CallShiftType.Custom)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the assigned shift is custom."));
        }
        date.Hours = request.Hours ?? request.CallType.GetHours();

        // evaluate rule violations, adminOverride is true -> only admin create dates
        ViolationResult violationResult;
        try
        {
            violationResult = await _ruleViolationService.EvaluateConstraints(request.ScheduleId,
                resident.ResidentId, date.ShiftDate, request.CallType);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure(ex.Message));
        }

        ViolationResultResponse response = new(violationResult);

        if (violationResult.IsViolation)
        {
            if (request.AdminOverride != true)
            {
                return Conflict(DateValidationResponse.ViolationFailure(response,
                    "The provided date violates constraints"));
            }

            if (violationResult.IsOverridable == false)
            {
                return Conflict(DateValidationResponse.ViolationFailure(response,
                    "The provided date violates non-overridable constraints"));
            }
        }

        _context.Dates.Add(date);
        await _context.SaveChangesAsync();

        return Created();
    }

    // POST: api/dates/new/validate
    [HttpPost("new/validate")]
    public async Task<ActionResult<DateValidationResponse>> ValidateCreateDate([FromBody] DateCreateRequest request)
    {
        Resident? resident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentId == request.ResidentId);

        if (resident?.GraduateYr is null)
        {
            return BadRequest(
                DateValidationResponse.NonViolationFailure("Resident with active PGY status not found"));
        }

        if (request.Hours is null && request.CallType == CallShiftType.Custom)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the assigned shift is custom."));
        }

        // evaluate rule violations, adminOverride is true -> only admin create dates
        ViolationResult violationResult;
        try
        {
            violationResult = await _ruleViolationService.EvaluateConstraints(request.ScheduleId,
                resident.ResidentId, request.ShiftDate, request.CallType);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure(ex.Message));
        }

        ViolationResultResponse response = new(violationResult);

        if (violationResult.IsViolation)
        {
            return Ok(DateValidationResponse.ViolationFailure(response,
                "The provided date violates constraints"));
        }

        if (request.Hours is null && CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(request.ShiftDate, resident.GraduateYr.Value) is null)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the assigned shift is not an algorithm shift."));
        }

        return Ok(DateValidationResponse.NoViolations());
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
            [FromQuery] string resident_id,
            [FromQuery] DateOnly date)
    {
        Resident? resident = await _context.Residents.FindAsync(resident_id);

        if (resident?.GraduateYr is null)
        {
            return BadRequest(new GenericResponse()
            {
                Success = false,
                Message = "Resident not found"
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

    // POST /api/dates/{id}/validate
    [HttpPost("{id}/validate")]
    public async Task<ActionResult> ValidateDateUpdateViolations(
        Guid id,
        [FromBody] DateUpdateRequest dateUpdateRequest)
    {
        Date? existingDate = await _context.Dates.Include(d => d.Resident).FirstOrDefaultAsync(d => d.DateId == id);
        if (existingDate == null)
        {
            return NotFound(DateValidationResponse.NonViolationFailure("Shift could not be found"));
        }

        bool isResidentUpdate = !string.IsNullOrEmpty(dateUpdateRequest.ResidentId) && dateUpdateRequest.ResidentId != existingDate.ResidentId;
        bool isDateOnlyUpdate = dateUpdateRequest.ShiftDate.HasValue && existingDate.ShiftDate != dateUpdateRequest.ShiftDate;

        // Update fields
        _dateConverter.UpdateDateFromDateUpdateRequest(existingDate, dateUpdateRequest);

        if (dateUpdateRequest.Hours is null && dateUpdateRequest.CallType == CallShiftType.Custom)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the updated shift is custom."));
        }

        if (dateUpdateRequest.Hours is not null)
        {
            existingDate.Hours = dateUpdateRequest.Hours.Value;
        }
        else if (dateUpdateRequest.CallType is not null)
        {
            existingDate.Hours = dateUpdateRequest.CallType.Value.GetHours();
        }

        Resident? resident
            = await _context.Residents.FirstOrDefaultAsync(r =>
                r.ResidentId == existingDate.ResidentId);
        if (resident?.GraduateYr == null)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Resident with active PGY status not found"));
        }

        // evaluate rule violations of update
        ViolationResult violationResult;
        try
        {
            violationResult = await _ruleViolationService.EvaluateConstraints(existingDate.ScheduleId, resident.ResidentId, existingDate.ShiftDate, existingDate.CallType, isDateOnlyUpdate, isResidentUpdate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure(ex.Message));
        }

        ViolationResultResponse response = new(violationResult);

        if (violationResult.IsViolation)
        {
            return Ok(DateValidationResponse.ViolationFailure(response,
                "The provided date violates constraints"));
        }

        if (dateUpdateRequest.Hours is null && CallShiftTypeExtensions.GetAlgorithmCallShiftTypeForDate(existingDate.ShiftDate, resident.GraduateYr.Value) is null)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the assigned shift is not an algorithm shift."));
        }

        return Ok(DateValidationResponse.NoViolations());
    }

    // PUT: api/dates/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDate(Guid id,
        [FromQuery] DateUpdateRequest dateUpdateRequest)
    {
        Date? existingDate = await _context.Dates.Include(d => d.Resident).FirstOrDefaultAsync(d => d.DateId == id);
        if (existingDate == null)
        {
            return NotFound(DateValidationResponse.NonViolationFailure("Shift could not be found"));
        }

        bool isResidentUpdate = !string.IsNullOrEmpty(dateUpdateRequest.ResidentId) && dateUpdateRequest.ResidentId != existingDate.ResidentId;
        bool isDateOnlyUpdate = dateUpdateRequest.ShiftDate.HasValue && existingDate.ShiftDate != dateUpdateRequest.ShiftDate;

        // Update fields
        _dateConverter.UpdateDateFromDateUpdateRequest(existingDate, dateUpdateRequest);

        if (dateUpdateRequest.Hours is null && dateUpdateRequest.CallType == CallShiftType.Custom)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure("Hours is required if the updated shift is custom."));
        }

        if (dateUpdateRequest.Hours is not null)
        {
            existingDate.Hours = dateUpdateRequest.Hours.Value;
        }
        else if (dateUpdateRequest.CallType is not null)
        {
            existingDate.Hours = dateUpdateRequest.CallType.Value.GetHours();
        }

        Resident? resident
            = await _context.Residents.FirstOrDefaultAsync(r =>
                r.ResidentId == existingDate.ResidentId);
        if (resident?.GraduateYr == null)
        {
            return BadRequest(
                DateValidationResponse.NonViolationFailure("Resident with active PGY status not found"));
        }

        // evaluate rule violations of update
        ViolationResult violationResult;
        try
        {
            violationResult = await _ruleViolationService.EvaluateConstraints(existingDate.ScheduleId, resident.ResidentId, existingDate.ShiftDate, existingDate.CallType, isDateOnlyUpdate, isResidentUpdate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(DateValidationResponse.NonViolationFailure(ex.Message));
        }

        ViolationResultResponse response = new(violationResult);

        if (violationResult.IsViolation)
        {
            if (dateUpdateRequest.AdminOverride != true)
            {
                return Conflict(DateValidationResponse.ViolationFailure(response,
                    "The provided date violates constraints"));
            }

            if (violationResult.IsOverridable == false)
            {
                return Conflict(DateValidationResponse.ViolationFailure(response,
                    "The provided date violates non-overridable constraints"));
            }
        }

        await _context.SaveChangesAsync();
        await _context.Entry(existingDate).ReloadAsync();
        return Ok(_dateConverter.CreateDateResponseFromDate(existingDate)); // returns updated object
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