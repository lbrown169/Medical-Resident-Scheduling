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
public class AnnouncementsController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly AnnouncementConverter _announcementConverter;
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(MedicalContext context, AnnouncementConverter announcementConverter, ILogger<AnnouncementsController> logger)
    {
        _context = context;
        _announcementConverter = announcementConverter;
        _logger = logger;
    }

    // POST: api/announcements
    [HttpPost]
    public async Task<IActionResult> CreateAnnouncement(
        [FromBody] AnnouncementCreateRequest announcementCreateRequest)
    {
        Announcement announcement = _announcementConverter.CreateAnnouncementFromAnnouncementCreateRequest(announcementCreateRequest);

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAnnouncement),
            new { id = announcement.AnnouncementId },
            _announcementConverter.CreateAnnouncementResponseFromAnnouncement(announcement)
        );
    }

    // GET: api/announcements
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnnouncementResponse>>> GetAllAnnouncements([FromQuery] string? authorId)
    {
        IQueryable<Announcement> query = _context.Announcements.AsQueryable();

        if (!string.IsNullOrEmpty(authorId))
        {
            query = query.Where(a => a.AuthorId == authorId);
        }

        List<AnnouncementResponse> announcements = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => _announcementConverter.CreateAnnouncementResponseFromAnnouncement(a))
            .ToListAsync();

        return Ok(announcements);
    }

    // GET: api/announcement/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<AnnouncementResponse>>> GetAnnouncement(Guid id)
    {
        IQueryable<Announcement> query = _context.Announcements.AsQueryable();

        Announcement? ann = await _context.Announcements.FindAsync(id);

        if (ann == null)
        {
            return NotFound();
        }

        return Ok(_announcementConverter.CreateAnnouncementResponseFromAnnouncement(ann));
    }

    // PUT: api/announcements/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnnouncement(Guid id,
        [FromBody] AnnouncementUpdateRequest announcementUpdateRequest)
    {
        Announcement? existing
            = await _context.Announcements.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        _announcementConverter.UpdateAnnouncementFromAnnouncementUpdateRequest(existing, announcementUpdateRequest);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(_announcementConverter.CreateAnnouncementResponseFromAnnouncement(existing));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update anouncement");
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
            return NotFound();
        }

        _context.Announcements.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}