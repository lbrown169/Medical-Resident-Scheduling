using MedicalDemo.Models;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

// TODO: This controller has not been refactored from the backend rewrite
[ApiController]
[Route("api/[controller]")]
public class RotationsController : ControllerBase
{
    private readonly MedicalContext _context;

    public RotationsController(MedicalContext context)
    {
        _context = context;
    }

    // POST: api/rotations
    [HttpPost]
    public async Task<IActionResult> CreateRotation(
        [FromBody] Rotation rotation)
    {
        if (rotation == null)
        {
            return BadRequest("Rotation object is null.");
        }

        if (rotation.RotationId == Guid.Empty)
        {
            rotation.RotationId = Guid.NewGuid();
        }

        _context.Rotations.Add(rotation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(FilterRotations),
            new { id = rotation.RotationId }, rotation);
    }

    // GET: api/rotations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rotation>>> GetAllRotations()
    {
        return await _context.Rotations.ToListAsync();
    }

    // GET: api/rotations/filter?residentId=&month=&rotation=
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Rotation>>> FilterRotations(
        [FromQuery] string? residentId,
        [FromQuery] string? month,
        [FromQuery] string? rotation)
    {
        IQueryable<Rotation> query = _context.Rotations.AsQueryable();

        if (!string.IsNullOrEmpty(residentId))
        {
            query = query.Where(v => v.ResidentId == residentId);
        }

        if (!string.IsNullOrEmpty(month))
        {
            query = query.Where(v => v.Month == month);
            ;
        }

        if (!string.IsNullOrEmpty(rotation))
        {
            query = query.Where(v => v.Rotation1 == rotation);
        }

        List<Rotation> results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No matching rotation records found.");
        }

        return Ok(results);
    }

    // PUT: api/rotations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRotation(Guid id,
        [FromBody] Rotation updatedRotation)
    {
        if (id != updatedRotation.RotationId)
        {
            return BadRequest("Rotation ID in URL and body do not match.");
        }

        Rotation? existingRotation
            = await _context.Rotations.FindAsync(id);
        if (existingRotation == null)
        {
            return NotFound("Rotation not found.");
        }

        // Update the fields
        existingRotation.ResidentId = updatedRotation.ResidentId;
        existingRotation.Month = updatedRotation.Month;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingRotation); // returns updated object
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"An error occurred while updating the date: {ex.Message}");
        }
    }

    // DELETE: api/rotations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRotation(Guid id)
    {
        Rotation? rotation = await _context.Rotations.FindAsync(id);

        if (rotation == null)
        {
            return NotFound("Rotation not found.");
        }

        _context.Rotations.Remove(rotation);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
}