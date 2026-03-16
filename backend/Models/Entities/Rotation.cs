using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities
{
    public partial class Rotation
    {
        public Guid RotationId { get; set; }

        public int AcademicYear { get; set; }

        public MonthOfYear AcademicMonthIndex { get; set; }

        [ForeignKey(nameof(Pgy4RotationSchedule))]
        public Guid? Pgy4RotationScheduleId { get; set; }

        public string? ResidentId { get; set; }

        public string Month { get; set; } = null!;

        public int PgyYear { get; set; }

        [ForeignKey(nameof(RotationType))]
        public Guid RotationTypeId { get; set; }

        public RotationType RotationType { get; set; } = null!;

        public virtual Resident Resident { get; set; } = null!;
    }
}