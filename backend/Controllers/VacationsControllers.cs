using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacationsController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly VacationConverter _vacationConverter;
    private readonly ILogger<VacationsController> _logger;

    public VacationsController(ILogger<VacationsController> logger, MedicalContext context, VacationConverter vacationConverter)
    {
        _logger = logger;
        _context = context;
        _vacationConverter = vacationConverter;
    }

    // POST: api/vacations
    [HttpPost]
    public async Task<IActionResult> CreateVacation(
        [FromBody] VacationCreateRequest vacationCreateRequest)
    {
        // Check if resident exists
        bool residentExists
            = await _context.Residents.AnyAsync(r =>
                r.ResidentId == vacationCreateRequest.ResidentId);
        if (!residentExists)
        {
            return BadRequest(
                $"Resident with id '{vacationCreateRequest.ResidentId}' does not exist.");
        }

        Vacation vacation = _vacationConverter.CreateVacationFromVacationCreateRequest(vacationCreateRequest);

        _context.Vacations.Add(vacation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVacation),
            new { id = vacation.VacationId }, _vacationConverter.CreateVacationResponseFromVacation(vacation));
    }

    // GET: api/vacations?residentId=&date=&reason=&status=
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VacationWithResidentResponse>>> GetAllVacations(
        [FromQuery] string? residentId,
        [FromQuery] DateOnly? date,
        [FromQuery] string? reason,
        [FromQuery] string? status
    ) {
        IQueryable<VacationWithResidentResponse> query = _context.Vacations
            .Select(v=> new VacationWithResidentResponse
                {
                    VacationId = v.VacationId,
                    ResidentId = v.ResidentId,
                    FirstName = v.Resident.FirstName,
                    LastName = v.Resident.LastName,
                    Date = v.Date,
                    Reason = v.Reason ?? string.Empty,
                    Status = v.Status,
                    Details = v.Details,
                    GroupId = v.GroupId,
                    HalfDay = v.HalfDay
                })
            ;

        if (!string.IsNullOrEmpty(residentId))
        {
            query = query.Where(v => v.ResidentId == residentId);
        }

        if (date.HasValue)
        {
            query = query.Where(v => v.Date == date.Value);
        }

        if (!string.IsNullOrEmpty(reason))
        {
            query = query.Where(v => v.Reason == reason);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(v => v.Status == status);
        }

        List<VacationWithResidentResponse> results = await query.ToListAsync();

        return Ok(results);
    }

    // GET: api/vacation/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<VacationWithResidentResponse>>> GetVacation(Guid id)
    {
        VacationWithResidentResponse? result = await _context.Vacations
            .Where(v => v.VacationId == id)
            .Select(v => new VacationWithResidentResponse
            {
                VacationId = v.VacationId,
                ResidentId = v.ResidentId,
                FirstName = v.Resident.FirstName,
                LastName = v.Resident.LastName,
                Date = v.Date,
                Reason = v.Reason ?? string.Empty,
                Status = v.Status,
                Details = v.Details,
                GroupId = v.GroupId,
                HalfDay = v.HalfDay
            })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // PUT: api/vacations/group/{groupId}/status
    [HttpPut("group/{groupId}/status/approve")]
    public async Task<IActionResult> ApproveVacationByGroup(string groupId)
    {
        List<Vacation> matchingRequests = await _context.Vacations
            .Where(v => v.GroupId == groupId)
            .ToListAsync();

        if (matchingRequests.Count == 0)
        {
            return NotFound();
        }

        foreach (Vacation request in matchingRequests)
        {
            request.Status = nameof(RequestStatus.Approved);
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    // PUT: api/vacations/group/{groupId}/status
    [HttpPut("group/{groupId}/status/deny")]
    public async Task<IActionResult> DenyVacationByGroup(string groupId)
    {
        List<Vacation> matchingRequests = await _context.Vacations
            .Where(v => v.GroupId == groupId)
            .ToListAsync();

        if (matchingRequests.Count == 0)
        {
            return NotFound();
        }

        foreach (Vacation request in matchingRequests)
        {
            request.Status = nameof(RequestStatus.Denied);
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    // PUT: api/vacations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacation(Guid id,
        [FromBody] VacationUpdateRequest update)
    {
        Vacation? existingVacation
            = await _context.Vacations.FindAsync(id);

        if (existingVacation == null)
        {
            return NotFound();
        }

        _vacationConverter.UpdateVacationFromVacationUpdateRequest(existingVacation, update);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_vacationConverter.CreateVacationResponseFromVacation(existingVacation)); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update vacation");
            return StatusCode(500, new GenericResponse
            {
                Success = false,
                Message = $"An error occurred while updating the date: {ex.Message}"
            });
        }
    }

    // DELETE: api/vacations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacation(Guid id)
    {
        Vacation? vacation = await _context.Vacations.FindAsync(id);

        if (vacation == null)
        {
            return NotFound();
        }

        _context.Vacations.Remove(vacation);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content
    }

    // DELETE: api/vacations/
    [HttpDelete]
    public async Task<IActionResult> DeleteAllVacations(
        [FromBody] List<Guid> vacationsIds)
    {
        // Fetch all vacations that exist in the database
        List<Vacation> vacationsToDelete = await _context.Vacations
            .Where(v => vacationsIds.Contains(v.VacationId))
            .ToListAsync();

        // Find which IDs were not found
        List<Guid> foundIds = vacationsToDelete.Select(v => v.VacationId).ToList();
        List<Guid> failedDeletedIds = vacationsIds.Except(foundIds).ToList();

        // Remove all found vacations in one operation
        _context.Vacations.RemoveRange(vacationsToDelete);
        await _context.SaveChangesAsync();

        VacationsNotDeletedResponse response = new VacationsNotDeletedResponse(){ notDeleted = failedDeletedIds };

        return Ok(response);
    }
}