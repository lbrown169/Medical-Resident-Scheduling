using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities
{
    public partial class Schedule
    {
        public Schedule()
        {
            Dates = new HashSet<Date>();
            SwapRequests = new HashSet<SwapRequest>();
        }

        public Guid ScheduleId { get; set; }
        public ScheduleStatus Status { get; set; }
        public int GeneratedYear { get; set; }
        public Semester Semester { get; set; }

        public virtual ICollection<Date> Dates { get; set; }
        public virtual ICollection<SwapRequest> SwapRequests { get; set; }
    }
}