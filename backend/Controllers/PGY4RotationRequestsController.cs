using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PGY4RotationRequestsController : ControllerBase
{
    private readonly MedicalContext _context;

    public PGY4RotationRequestsController(MedicalContext context)
    {
        _context = context;
    }

    // GET: api/PGY4RotationRequests/{residentId}
    [HttpGet("{residentId}")]
    public async Task<ActionResult<PGY4RotationRequests>> GetRequest(string residentId)
    {
        var request = await _context.PGY4RotationRequests
            .FirstOrDefaultAsync(r => r.ResidentId == residentId);

        if (request == null)
        {
            // Copying the style of returns from SwapRequests
            return NotFound("No rotation request found for resident.");
        }

        return Ok(request);
    }

    // POST: api/PGY4RotationRequests
    [HttpPost]
    public async Task<ActionResult<PGY4RotationRequests>> SubmitRequest([FromBody] PGY4RotationRequests newRequest)
    {
        if (newRequest == null || string.IsNullOrEmpty(newRequest.ResidentId))
        {
            return BadRequest("Invalid rotation request.");
        }

        // Delete existing request if it exists
        var existing = await _context.PGY4RotationRequests
            .FirstOrDefaultAsync(r => r.ResidentId == newRequest.ResidentId);

        if (existing != null)
        {
            _context.PGY4RotationRequests.Remove(existing);
        }

        // Generate a new Guid if not supplied
        if (newRequest.RequestId == Guid.Empty)
        {
            newRequest.RequestId = Guid.NewGuid();
        }

        _context.PGY4RotationRequests.Add(newRequest);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRequest), new { residentId = newRequest.ResidentId }, newRequest);
    }

    // PUT: api/PGY4RotationRequests/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequest(Guid id, [FromBody] PGY4RotationRequests updatedRequest)
    {
        if (id != updatedRequest.RequestId)
        {
            return BadRequest("ID in URL and body do not match.");
        }

        var existing = await _context.PGY4RotationRequests.FindAsync(id);
        if (existing == null)
        {
            return NotFound("Rotation request not found.");
        }

        // Update fields on update, do NOT update resident id
        existing.FirstPriority = updatedRequest.FirstPriority;
        existing.SecondPriority = updatedRequest.SecondPriority;
        existing.ThirdPriority = updatedRequest.ThirdPriority;
        existing.FourthPriority = updatedRequest.FourthPriority;
        existing.FifthPriority = updatedRequest.FifthPriority;
        existing.SixthPriority = updatedRequest.SixthPriority;
        existing.SeventhPriority = updatedRequest.SeventhPriority;
        existing.EighthPriority = updatedRequest.EighthPriority;
        existing.Alternative1 = updatedRequest.Alternative1;
        existing.Alternative2 = updatedRequest.Alternative2;
        existing.Avoid1 = updatedRequest.Avoid1;
        existing.Avoid2 = updatedRequest.Avoid2;
        existing.Avoid3 = updatedRequest.Avoid3;
        existing.AdditionalNotes = updatedRequest.AdditionalNotes;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }
}