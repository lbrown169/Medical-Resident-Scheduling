using System;
using System.Collections.Generic;

namespace MedicalDemo.Models
{
    public partial class Rotation
    {
        public Guid RotationId { get; set; }
        public string ResidentId { get; set; } = null!;
        public string Month { get; set; } = null!;
        public string Rotation1 { get; set; } = null!;

        public virtual Resident Resident { get; set; } = null!;
    }
}
