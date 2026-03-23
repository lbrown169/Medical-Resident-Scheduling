namespace MedicalDemo.Models.Entities
{
    public partial class Vacation
    {
        public Guid VacationId { get; set; }
        public string ResidentId { get; set; } = null!;
        public DateOnly Date { get; set; }
        public string? Reason { get; set; }
        // Status was already a string when they started inserting vacations, so we can't update this one
        public string Status { get; set; } = null!;
        public string? Details { get; set; }
        public string GroupId { get; set; } = null!;
        public string? HalfDay { get; set; }

        public virtual Resident Resident { get; set; } = null!;
    }
}