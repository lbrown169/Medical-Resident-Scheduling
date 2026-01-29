using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities
{
    public partial class SwapRequest
    {
        public Guid SwapRequestId { get; set; }
        public Guid ScheduleId { get; set; }
        public string RequesterId { get; set; } = null!;
        public string RequesteeId { get; set; } = null!;
        public DateOnly RequesterDate { get; set; }
        public DateOnly RequesteeDate { get; set; }
        public SwapRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Details { get; set; }

        public virtual Resident Requestee { get; set; } = null!;
        public virtual Resident Requester { get; set; } = null!;
        public virtual Schedule ScheduleSwap { get; set; } = null!;
    }
}
