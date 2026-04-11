using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SwapRequestsController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly SwapRequestConverter _swapRequestConverter;
    private readonly RuleViolationService _ruleViolationService;

    public SwapRequestsController(MedicalContext context, SwapRequestConverter swapRequestConverter, RuleViolationService ruleViolationService)
    {
        _context = context;
        _swapRequestConverter = swapRequestConverter;
        _ruleViolationService = ruleViolationService;
    }

    // POST: api/swaprequests
    [HttpPost]
    public async Task<IActionResult> CreateSwapRequest(
        [FromBody] SwapRequestCreateRequest swapCreateRequest)
    {
        SwapRequest swapRequest
            = _swapRequestConverter
                .CreateSwapRequestFromSwapRequestCreateRequest(
                    swapCreateRequest);

        SwapRequestValidateResponse swapRequestValidationResult = await ValidateSwapRequestAndAssignScheduleId(swapRequest);

        if (!swapRequestValidationResult.Success)
        {
            return BadRequest(swapRequestValidationResult);
        }

        _context.SwapRequests.Add(swapRequest);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSwapRequest),
            new { id = swapRequest.SwapRequestId }, _swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swapRequest));
    }

    // GET: api/swaprequests?schedule_swap_id=&requester_id=&requestee_id=&status=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwapRequestResponse>>> GetSwapRequests(
        [FromQuery] Guid? schedule_swap_id,
        [FromQuery] string? requester_id,
        [FromQuery] string? requestee_id,
        [FromQuery] RequestStatus? status)
    {
        IQueryable<SwapRequest>
            query = _context.SwapRequests.AsQueryable();

        if (schedule_swap_id.HasValue)
        {
            query = query.Where(s =>
                s.ScheduleId == schedule_swap_id.Value);
        }

        if (!string.IsNullOrEmpty(requester_id))
        {
            query = query.Where(s => s.RequesterId == requester_id);
        }

        if (!string.IsNullOrEmpty(requestee_id))
        {
            query = query.Where(s => s.RequesteeId == requestee_id);
        }

        if (status != null)
        {
            query = query.Where(s => s.Status == status);
        }

        List<SwapRequestResponse> results = await query.Select(sr => _swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(sr)).ToListAsync();

        return Ok(results);
    }

    // GET: api/swaprequests/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SwapRequestResponse>> GetSwapRequest(Guid id)
    {
        SwapRequest? swapRequest = await _context.SwapRequests.FindAsync(id);

        if (swapRequest == null)
        {
            return NotFound();
        }

        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swapRequest));
    }

    // DELETE: api/swaprequests/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSwapRequest(Guid id)
    {
        SwapRequest? existing = await _context.SwapRequests.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        _context.SwapRequests.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/swaprequests
    [HttpDelete]
    public async Task<IActionResult> DeleteAllSwapRequests(
        [FromBody] List<Guid> swapRequestIds)
    {
        // Fetch all swap requests that are marked as read
        List<SwapRequest> swapRequestsToDelete = await _context.SwapRequests
            .Where(s => swapRequestIds.Contains(s.SwapRequestId) && s.IsRead)
        .ToListAsync();

        // Find which IDs were not found (don't exist/pending status)
        List<Guid> foundIds = swapRequestsToDelete.Select(s => s.SwapRequestId).ToList();
        List<Guid> failedDeletedIds = swapRequestIds.Except(foundIds).ToList();

        // Remove all found swap requests in one operation
        _context.SwapRequests.RemoveRange(swapRequestsToDelete);
        await _context.SaveChangesAsync();

        SwapRequestsNotDeletedResponse response = new SwapRequestsNotDeletedResponse() { notDeleted = failedDeletedIds };

        return Ok(response);
    }

    // POST: api/swaprequests/{id}/approve
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveSwapRequest(Guid id)
    {
        SwapRequest? swap = await _context.SwapRequests.FindAsync(id);
        if (swap == null)
        {
            return NotFound();
        }

        if (swap.Status != RequestStatus.Pending)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = "SwapRequest is not pending."
            });
        }

        // Fetch all candidate dates for each resident/date
        Date? requesterDate = await _context.Dates
            .Where(d =>
                d.ResidentId == swap.RequesterId &&
                d.ShiftDate == swap.RequesterDate)
            .FirstOrDefaultAsync();
        Date? requesteeDate = await _context.Dates
            .Where(d =>
                d.ResidentId == swap.RequesteeId &&
                d.ShiftDate == swap.RequesteeDate)
            .FirstOrDefaultAsync();

        if (requesterDate == null || requesteeDate == null)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = "Could not find both shift dates to perform the swap."
            });
        }

        // Swap the resident IDs
        (requesteeDate.ResidentId, requesterDate.ResidentId) = (swap.RequesterId, swap.RequesteeId);
        swap.Status = RequestStatus.Approved;
        swap.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swap));
    }

    // POST: api/swaprequests/{id}/deny
    [HttpPost("{id}/deny")]
    public async Task<IActionResult> DenySwapRequest(Guid id)
    {
        SwapRequest? swap = await _context.SwapRequests.FindAsync(id);
        if (swap == null)
        {
            return NotFound();
        }

        if (swap.Status != RequestStatus.Pending)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = "SwapRequest is not pending."
            });
        }

        swap.Status = RequestStatus.Denied;
        swap.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Optionally: Add logic to notify users, etc.
        // Add recent activity for requester (handled in dashboard fetch for now)

        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swap));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<SwapRequestResponse>> UpdateSwapRequest(Guid id, [FromBody] UpdateSwapRequest swapRequestUpdates)
    {
        SwapRequest? swap = await _context.SwapRequests.FindAsync(id);
        if (swap == null)
        {
            return NotFound();
        }

        _swapRequestConverter.UpdateSwapRequestFromSwapRequestUpdates(swap, swapRequestUpdates);
        await _context.SaveChangesAsync();
        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swap));
    }

    private async Task<SwapRequestValidateResponse> ValidateSwapRequestAndAssignScheduleId(SwapRequest swapRequest)
    {
        // Fetch residents
        Resident? requester =
            await _context.Residents.FindAsync(swapRequest.RequesterId);
        Resident? requestee =
            await _context.Residents.FindAsync(swapRequest.RequesteeId);

        if (requester == null || requestee == null)
        {
            return new SwapRequestValidateResponse()
            {
                Success = false, Message = "Requester or requestee not found."
            };
        }

        // Fetch dates
        Date? requesterDate
            = await _context.Dates
                .FirstOrDefaultAsync(d =>
                d.ResidentId == swapRequest.RequesterId &&
                d.ShiftDate == swapRequest.RequesterDate &&
                d.Schedule.Status == ScheduleStatus.Published);
        Date? requesteeDate
            = await _context.Dates
                .FirstOrDefaultAsync(d =>
                d.ResidentId == swapRequest.RequesteeId &&
                d.ShiftDate == swapRequest.RequesteeDate &&
                d.Schedule.Status == ScheduleStatus.Published);
        if (requesterDate == null || requesteeDate == null)
        {
            return new SwapRequestValidateResponse()
            { Success = false, Message = "Could not find both shift dates for the swap." };
        }

        // Check shift type
        if (requesterDate.CallType != requesteeDate.CallType)
        {
            return new SwapRequestValidateResponse()
            {
                Success = false,
                Message = "Both shifts must be the same type (e.g., Sunday with Sunday, Saturday with Saturday, Short with Short)."
            };
        }

        if (requesterDate.ScheduleId != requesteeDate.ScheduleId)
        {
            return new SwapRequestValidateResponse()
            {
                Success = false,
                Message = "Both shifts must belong to the same schedule."
            };
        }

        // evaluate rule violations if swap occurs
        IndividualValidationResult requesterValidationResult;
        try
        {
            ViolationResult requesterViolationResult
                = await _ruleViolationService.EvaluateConstraints(requesteeDate.ScheduleId,
                    requester.ResidentId, requesteeDate.ShiftDate, requesteeDate.CallType);
            ViolationResultResponse requesterViolationResultResponse = new(requesterViolationResult);
            requesterValidationResult = new IndividualValidationResult() { Message = requesterViolationResultResponse.IsViolation ? "Requester constraint violations" : "No Violations", Violations = requesterViolationResultResponse };
        }
        catch (ArgumentException ex)
        {
            requesterValidationResult = new IndividualValidationResult() { Message = ex.Message, Violations = null };
        }

        IndividualValidationResult requesteeValidationResult;
        try
        {
            ViolationResult requesteeViolationResult = await _ruleViolationService.EvaluateConstraints(requesterDate.ScheduleId, requestee.ResidentId, requesterDate.ShiftDate, requesterDate.CallType);
            ViolationResultResponse requesteeViolationResultResponse = new(requesteeViolationResult);
            requesteeValidationResult = new IndividualValidationResult() { Message = requesteeViolationResultResponse.IsViolation ? "Requestee constraint violations" : "No Violations", Violations = requesteeViolationResultResponse };
        }
        catch (ArgumentException ex)
        {
            requesteeValidationResult = new IndividualValidationResult() { Message = ex.Message, Violations = null };
        }

        if (requesterValidationResult.Violations == null ||
            requesteeValidationResult.Violations == null)
        {
            return new SwapRequestValidateResponse
            {
                Success = false,
                Message = "Failed to process rule violations",
                Requester = requesterValidationResult,
                Requestee = requesteeValidationResult
            };
        }

        if (requesterValidationResult.Violations.IsViolation || requesteeValidationResult.Violations.IsViolation)
        {
            return new SwapRequestValidateResponse
            {
                Success = false,
                Message = "SwapRequest will result in rule violation(s) for requestee or requester",
                Requester = requesterValidationResult,
                Requestee = requesteeValidationResult
            };
        }

        swapRequest.ScheduleId = requesterDate.ScheduleId;

        return new SwapRequestValidateResponse { Success = true, Message = "No Constraint Violations", Requester = requesterValidationResult, Requestee = requesteeValidationResult };
    }
}