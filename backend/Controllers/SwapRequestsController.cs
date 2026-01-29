using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
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

    public SwapRequestsController(MedicalContext context, SwapRequestConverter swapRequestConverter)
    {
        _context = context;
        _swapRequestConverter = swapRequestConverter;
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

        string? message = await ValidateSwapRequestAndAssignScheduleId(swapRequest);

        if (message != null)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = message
            });
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
        [FromQuery] SwapRequestStatus? status)
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

    // PUT: api/swaprequests/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSwapRequest(Guid id,
        [FromBody] SwapRequestUpdateRequest updatedRequest)
    {
        SwapRequest? existing = await _context.SwapRequests.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (existing.Status == SwapRequestStatus.Approved)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = "Cannot update an approved swap request."
            });
        }

        _swapRequestConverter.UpdateSwapRequestFromSwapRequestUpdateRequest(existing, updatedRequest);
        string? message = await ValidateSwapRequestAndAssignScheduleId(existing);

        if (message != null)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = message
            });
        }

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(existing));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new GenericResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the swap request: {ex.Message}"
                }
            );
        }
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

    // POST: api/swaprequests/{id}/approve
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveSwapRequest(Guid id)
    {
        SwapRequest? swap = await _context.SwapRequests.FindAsync(id);
        if (swap == null)
        {
            return NotFound();
        }

        if (swap.Status != SwapRequestStatus.Pending)
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
        swap.Status = SwapRequestStatus.Approved;
        swap.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swap));
    }

    // POST: api/swaprequests/{id}/deny
    [HttpPost("{id}/deny")]
    public async Task<IActionResult> DenySwapRequest(Guid id,
        [FromBody] SwapRequestDenyRequest denyRequest)
    {
        SwapRequest? swap = await _context.SwapRequests.FindAsync(id);
        if (swap == null)
        {
            return NotFound();
        }

        if (swap.Status != SwapRequestStatus.Pending)
        {
            return BadRequest(new GenericResponse
            {
                Success = false,
                Message = "SwapRequest is not pending."
            });
        }

        swap.Status = SwapRequestStatus.Denied;
        swap.Details = denyRequest.Reason ?? "";
        swap.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Optionally: Add logic to notify users, etc.
        // Add recent activity for requester (handled in dashboard fetch for now)

        return Ok(_swapRequestConverter.CreateSwapRequestResponseFromSwapRequest(swap));
    }

    private async Task<string?> ValidateSwapRequestAndAssignScheduleId(SwapRequest swapRequest)
    {
                // Fetch residents
        Resident? requester =
            await _context.Residents.FindAsync(swapRequest.RequesterId);
        Resident? requestee =
            await _context.Residents.FindAsync(swapRequest.RequesteeId);

        if (requester == null || requestee == null)
        {
            return "Requester or requestee not found.";
        }

        // Check PGY (graduate_yr)
        if (requester.GraduateYr != requestee.GraduateYr)
        {
            return "Both residents must be the same PGY level to swap.";
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
            return "Could not find both shift dates for the swap.";
        }

        // Check shift type
        if (requesterDate.CallType != requesteeDate.CallType)
        {
            return
                "Both shifts must be the same type (e.g., Sunday with Sunday, Saturday with Saturday, Short with Short).";
        }

        if (requesterDate.ScheduleId != requesteeDate.ScheduleId)
        {
            return "Both shifts must belong to the same schedule.";
        }

        swapRequest.ScheduleId = requesterDate.ScheduleId;

        return null;
    }
}