using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

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
        [FromBody] Rotations rotation)
    {
        if (rotation == null)
        {
            return BadRequest("Rotation object is null.");
        }

        if (rotation.RotationId == Guid.Empty)
        {
            rotation.RotationId = Guid.NewGuid();
        }

        _context.rotations.Add(rotation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(FilterRotations),
            new { id = rotation.RotationId }, rotation);
    }

    // GET: api/rotations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rotations>>> GetAllRotations()
    {
        return await _context.rotations.ToListAsync();
    }

    // GET: api/rotations/filter?residentId=&month=&rotation=
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Rotations>>> FilterRotations(
        [FromQuery] string? residentId,
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? pgyYear,
        [FromQuery] Guid? rotationTypeId)
    {
        IQueryable<Rotations> query = _context.rotations.AsQueryable();

        if (!string.IsNullOrEmpty(residentId))
        {
            query = query.Where(v => v.ResidentId == residentId);
        }

        if (!string.IsNullOrEmpty(month))
        {
            query = query.Where(v => v.Month == month);
            ;
        }

        // Add query logic for year
        // Add query logic for pgyYear

        if (rotationTypeId != null)
        {
            query = query.Where(v => v.RotationTypeId == rotationTypeId);
        }

        List<Rotations> results = await query.ToListAsync();

        if (results.Count == 0)
        {
            return NotFound("No matching rotation records found.");
        }

        return Ok(results);
    }

    // PUT: api/rotations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRotation(Guid id,
        [FromBody] Rotations updatedRotation)
    {
        if (id != updatedRotation.RotationId)
        {
            return BadRequest("Rotation ID in URL and body do not match.");
        }

        Rotations? existingRotation
            = await _context.rotations.FindAsync(id);
        if (existingRotation == null)
        {
            return NotFound("Rotation not found.");
        }

        // Update the fields
        existingRotation.ResidentId = updatedRotation.ResidentId;
        existingRotation.Month = updatedRotation.Month;
        existingRotation.RotationTypeId = updatedRotation.RotationTypeId;

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
        Rotations? rotation = await _context.rotations.FindAsync(id);

        if (rotation == null)
        {
            return NotFound("Rotation not found.");
        }

        _context.rotations.Remove(rotation);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
}