using System;
using System.Collections.Generic;

namespace MedicalDemo.Models
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
