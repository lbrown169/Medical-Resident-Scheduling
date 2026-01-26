using System;
using System.Collections.Generic;

namespace MedicalDemo.Models
{
    public partial class Blackout
    {
        public Guid BlackoutId { get; set; }
        public string ResidentId { get; set; } = null!;
        public DateOnly Date { get; set; }

        public virtual Resident Resident { get; set; } = null!;
    }
}
