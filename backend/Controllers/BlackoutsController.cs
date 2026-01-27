using MedicalDemo.Converters;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlackoutsController : ControllerBase
{
    private readonly BlackoutConverter _blackoutConverter;
    private readonly MedicalContext _context;

    public BlackoutsController(MedicalContext context, BlackoutConverter blackoutConverter)
    {
        _context = context;
        _blackoutConverter = blackoutConverter;
    }

    // POST: api/blackouts
    [HttpPost]
    public async Task<IActionResult> CreateBlackout(
        [FromBody] BlackoutCreateRequest blackoutCreateRequest)
    {
        Blackout blackout = _blackoutConverter.CreateBlackoutFromBlackoutCreateRequest(blackoutCreateRequest);
        _context.Blackouts.Add(blackout);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBlackouts),
            new { resident_id = blackout.BlackoutId }, _blackoutConverter.CreateBlackoutResponseFromBlackout(blackout));
    }

    // GET: api/blackouts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlackoutResponse>>> GetBlackouts(
        [FromQuery] string? resident_id,
        [FromQuery] DateOnly? date
    )
    {
        IQueryable<Blackout> query = _context.Blackouts.AsQueryable();

        if (!string.IsNullOrEmpty(resident_id))
        {
            query = query.Where(b => b.ResidentId == resident_id);
        }

        if (date.HasValue)
        {
            query = query.Where(b => b.Date == date.Value);
        }

        List<BlackoutResponse> results = await query.Select(b => _blackoutConverter.CreateBlackoutResponseFromBlackout(b)).ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No blackouts matched the filter criteria.");
        }

        return Ok(results);
    }

    // PUT: api/blackouts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlackout(Guid id,
        [FromBody] BlackoutUpdateRequest updatedBlackout)
    {
        Blackout? existingBlackout
            = await _context.Blackouts.FindAsync(id);
        if (existingBlackout == null)
        {
            return NotFound("Blackout not found.");
        }

        // Update fields
        _blackoutConverter.UpdateBlackoutFromBlackoutUpdateRequest(existingBlackout, updatedBlackout);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_blackoutConverter.CreateBlackoutResponseFromBlackout(existingBlackout));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // DELETE: api/blackouts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlackout(Guid id)
    {
        Blackout? blackout = await _context.Blackouts.FindAsync(id);
        if (blackout == null)
        {
            return NotFound("Blackout not found.");
        }

        _context.Blackouts.Remove(blackout);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
}