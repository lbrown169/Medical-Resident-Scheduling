namespace MedicalDemo.Models.Entities
{
    public partial class Blackout
    {
        public Guid BlackoutId { get; set; }
        public string ResidentId { get; set; } = null!;
        public DateOnly Date { get; set; }

        public virtual Resident Resident { get; set; } = null!;
    }
}
