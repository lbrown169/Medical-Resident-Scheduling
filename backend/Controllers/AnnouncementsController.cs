using MedicalDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly MedicalContext _context;

    public AnnouncementsController(MedicalContext context)
    {
        _context = context;
    }

    // POST: api/announcements
    [HttpPost]
    public async Task<IActionResult> CreateAnnouncement(
        [FromBody] Announcement announcement)
    {
        if (announcement == null)
        {
            return BadRequest("Announcement object is null.");
        }

        if (announcement.AnnouncementId == Guid.Empty)
        {
            announcement.AnnouncementId = Guid.NewGuid();
        }

        // Set default author if not provided
        if (string.IsNullOrEmpty(announcement.AuthorId))
        {
            announcement.AuthorId = "Admin";
        }

        announcement.CreatedAt = DateTime.UtcNow;

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(FilterAnnouncements),
            new { id = announcement.AnnouncementId }, announcement);
    }

    // GET: api/announcements
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Announcement>>>
        GetAllAnnouncements()
    {
        List<Announcement> announcements = await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(announcements);
    }

    // GET: api/announcements/filter?author_id=
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Announcement>>>
        FilterAnnouncements(
            [FromQuery] string author_id)
    {
        IQueryable<Announcement> query
            = _context.Announcements.AsQueryable();

        if (!string.IsNullOrEmpty(author_id))
        {
            query = query.Where(a => a.AuthorId == author_id);
        }

        List<Announcement> results
            = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

        if (!results.Any())
        {
            return NotFound("No announcements matched the filter.");
        }

        return Ok(results);
    }

    // PUT: api/announcements/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnnouncement(Guid id,
        [FromBody] Announcement updated)
    {
        if (id != updated.AnnouncementId)
        {
            return BadRequest("ID mismatch between URL and body.");
        }

        Announcement? existing
            = await _context.Announcements.FindAsync(id);
        if (existing == null)
        {
            return NotFound("Announcement not found.");
        }

        existing.Message = updated.Message;
        existing.AuthorId = updated.AuthorId;
        // You typically wouldn't update CreatedAt, so we leave it as-is

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existing);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500,
                $"Error updating announcement: {ex.Message}");
        }
    }

    // DELETE: api/announcements/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnnouncement(Guid id)
    {
        Announcement? existing
            = await _context.Announcements.FindAsync(id);
        if (existing == null)
        {
            return NotFound("Announcement not found.");
        }

        _context.Announcements.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}