using System.Text.Json.Serialization;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities
{
    public partial class Resident
    {
        public Resident()
        {
            Blackouts = new HashSet<Blackout>();
            Dates = new HashSet<Date>();
            Rotations = new HashSet<Rotation>();
            SwapRequestRequestees = new HashSet<SwapRequest>();
            SwapRequestRequesters = new HashSet<SwapRequest>();
        }

        public string ResidentId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int GraduateYr { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNum { get; set; } = null!;
        public int WeeklyHours { get; set; }
        public int TotalHours { get; set; }
        public int BiYearlyHours { get; set; }
        public int? HospitalRoleProfile { get; set; }

        public ChiefType ChiefType { get; set; }

        public virtual ICollection<Blackout> Blackouts { get; set; }
        public virtual ICollection<Date> Dates { get; set; }
        [JsonIgnore]
        public virtual ICollection<Rotation> Rotations { get; set; }
        public virtual ICollection<SwapRequest> SwapRequestRequestees { get; set; }
        public virtual ICollection<SwapRequest> SwapRequestRequesters { get; set; }
        public virtual ICollection<Vacation> Vacations { get; set; }
    }
}