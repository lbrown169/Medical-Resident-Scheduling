using System;
using System.Collections.Generic;

namespace MedicalDemo.Models
{
    public partial class Vacation
    {
        public Guid VacationId { get; set; }
        public string ResidentId { get; set; } = null!;
        public DateOnly Date { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = null!;
        public string? Details { get; set; }
        public string GroupId { get; set; } = null!;
        public string? HalfDay { get; set; }
    }
}
