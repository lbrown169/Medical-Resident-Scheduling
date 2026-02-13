using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models.Entities
{
    public partial class Rotation
    {
        public Guid RotationId { get; set; }

        [ForeignKey(nameof(PGY4RotationSchedule))]
        public Guid? Pgy4RotationScheduleId { get; set; }

        public string ResidentId { get; set; } = null!;

        public string Month { get; set; } = null!;

        public int MonthIndex { get; set; }

        // Keeping this here for now, we can remove it later if we want
        public string Rotation1 { get; set; } = null!;

        public int PgyYear { get; set; }

        [ForeignKey(nameof(RotationType))]
        public Guid RotationTypeId { get; set; }

        public RotationType RotationType { get; set; } = null!;

        public virtual Resident Resident { get; set; } = null!;
    }
}