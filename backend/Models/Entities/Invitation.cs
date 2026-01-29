namespace MedicalDemo.Models.Entities
{
    public partial class Invitation
    {
        public string Token { get; set; } = null!;
        public string? ResidentId { get; set; }
        public DateTime? Expires { get; set; }
        public bool Used { get; set; }
    }
}