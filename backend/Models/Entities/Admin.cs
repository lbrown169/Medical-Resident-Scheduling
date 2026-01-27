namespace MedicalDemo.Models.Entities
{
    public partial class Admin
    {
        public Admin()
        {
            Announcements = new HashSet<Announcement>();
        }

        public string AdminId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNum { get; set; } = null!;

        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}
