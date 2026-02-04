using System.Globalization;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class AnnouncementConverter
{
    public Announcement CreateAnnouncementFromAnnouncementCreateRequest(
        AnnouncementCreateRequest announcementCreateRequest)
    {
        Announcement announcement = new()
        {
            AnnouncementId = Guid.NewGuid(),
            AuthorId = announcementCreateRequest.authorId ?? "Admin",
            CreatedAt = DateTime.UtcNow,
            Message = announcementCreateRequest.message
        };

        return announcement;
    }

    public AnnouncementResponse
        CreateAnnouncementResponseFromAnnouncement(
            Announcement announcement)
    {
        return new AnnouncementResponse()
        {
            announcementId = announcement.AnnouncementId.ToString(),
            message = announcement.Message,
            createdAt = announcement.CreatedAt.ToString(CultureInfo.InvariantCulture),
        };
    }

    public void UpdateAnnouncementFromAnnouncementUpdateRequest(
        Announcement announcement,
        AnnouncementUpdateRequest announcementUpdateRequest)
    {
        announcement.AuthorId = announcementUpdateRequest.AuthorId ?? announcement.AuthorId;
        announcement.Message = announcementUpdateRequest.Message ?? announcement.Message;
    }
}