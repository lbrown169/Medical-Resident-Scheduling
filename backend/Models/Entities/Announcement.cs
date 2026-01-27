namespace MedicalDemo.Models.Entities
{
    public partial class Announcement
    {
        public Guid AnnouncementId { get; set; }
        public string AuthorId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual Admin Author { get; set; } = null!;
    }
}
