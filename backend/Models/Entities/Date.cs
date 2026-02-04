using MedicalDemo.Algorithm;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Models.Entities
{
    public partial class Date
    {
        public Guid DateId { get; set; }
        public Guid ScheduleId { get; set; }
        public string ResidentId { get; set; } = null!;
        public DateOnly ShiftDate { get; set; }
        public CallShiftType CallType { get; set; }
        public int Hours { get; set; }

        public virtual Resident Resident { get; set; } = null!;
        public virtual Schedule Schedule { get; set; } = null!;
    }
}