using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class VacationsController : ControllerBase
{
    private readonly MedicalContext _context;

    public VacationsController(MedicalContext context)
    {
        _context = context;
    }

    // POST: api/vacations
    [HttpPost]
    public async Task<IActionResult> CreateVacation(
        [FromBody] Vacation vacation)
    {
        if (vacation == null)
        {
            return BadRequest("Vacation object is null.");
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(vacation.ResidentId) ||
            vacation.Date == default ||
            string.IsNullOrWhiteSpace(vacation.Reason) ||
            string.IsNullOrWhiteSpace(vacation.Status))
        {
            return BadRequest(
                "Missing required fields: ResidentId, Date, Reason, and Status are required.");
        }

        // Check if resident exists
        bool residentExists
            = await _context.Residents.AnyAsync(r =>
                r.ResidentId == vacation.ResidentId);
        if (!residentExists)
        {
            return BadRequest(
                $"Resident with id '{vacation.ResidentId}' does not exist.");
        }

        // Generate a new Guid if not supplied
        if (vacation.VacationId == Guid.Empty)
        {
            vacation.VacationId = Guid.NewGuid();
        }

        _context.Vacations.Add(vacation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(FilterVacations),
            new { id = vacation.VacationId }, vacation);
    }

    // GET: api/vacations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VacationWithResidentDto>>>
        GetAllVacations()
    {
        List<VacationWithResidentDto> vacations = await _context.Vacations
            .Join(_context.Residents,
                v => v.ResidentId,
                r => r.ResidentId,
                (v, r) => new VacationWithResidentDto
                {
                    VacationId = v.VacationId,
                    ResidentId = r.ResidentId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Date = v.Date,
                    Reason = v.Reason,
                    Status = v.Status,
                    Details = v.Details,
                    GroupId = v.GroupId
                })
            .ToListAsync();

        return Ok(vacations);
    }

    // PUT: api/vacations/group/{groupId}/status
    [HttpPut("group/{groupId}/status")]
    public async Task<IActionResult> UpdateStatusByGroup(string groupId,
        [FromBody] UpdateStatusDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Status))
        {
            return BadRequest("Status is required.");
        }

        List<Vacation> matchingRequests = await _context.Vacations
            .Where(v => v.GroupId == groupId)
            .ToListAsync();

        if (!matchingRequests.Any())
        {
            return NotFound(
                $"No vacation requests found for groupId '{groupId}'.");
        }

        foreach (Vacation request in matchingRequests)
        {
            request.Status = input.Status;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message
                = $"Status updated to '{input.Status}' for groupId '{groupId}'."
        });
    }


    // GET: api/vacations/filter?residentId=&date=&reason=&status=
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<VacationWithResidentDto>>>
        FilterVacations(
            [FromQuery] string? residentId,
            [FromQuery] DateOnly? date,
            [FromQuery] string? reason,
            [FromQuery] string? status)
    {
        IQueryable<VacationWithResidentDto> query = _context.Vacations
            .Join(_context.Residents,
                v => v.ResidentId,
                r => r.ResidentId,
                (v, r) => new VacationWithResidentDto
                {
                    VacationId = v.VacationId,
                    ResidentId = v.ResidentId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Date = v.Date,
                    Reason = v.Reason,
                    Status = v.Status,
                    Details = v.Details,
                    GroupId = v.GroupId
                });

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

        List<VacationWithResidentDto> results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No matching vacation records found.");
        }

        return Ok(results);
    }

    // PUT: api/vacations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVacation(Guid id,
        [FromBody] Vacation updatedVacation)
    {
        if (id != updatedVacation.VacationId)
        {
            return BadRequest("Vacation ID in URL and body do not match.");
        }

        Vacation? existingVacation
            = await _context.Vacations.FindAsync(id);

        if (existingVacation == null)
        {
            return NotFound("Vacation not found.");
        }

        // Update the fields
        existingVacation.ResidentId = updatedVacation.ResidentId;
        existingVacation.Date = updatedVacation.Date;
        existingVacation.Reason = updatedVacation.Reason;
        existingVacation.Status = updatedVacation.Status;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingVacation); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // DELETE: api/vacations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVacation(Guid id)
    {
        Vacation? vacation = await _context.Vacations.FindAsync(id);

        if (vacation == null)
        {
            return NotFound("Vacation not found.");
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

        var response = new { notDeleted = failedDeletedIds };

        return Ok(response);
    }
}