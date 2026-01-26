using System;
using System.Collections.Generic;

namespace MedicalDemo.Models
{
    public partial class SwapRequest
    {
        public Guid IdswapRequests { get; set; }
        public Guid ScheduleSwapId { get; set; }
        public string RequesterId { get; set; } = null!;
        public string RequesteeId { get; set; } = null!;
        public DateOnly RequesterDate { get; set; }
        public DateOnly RequesteeDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Details { get; set; }

        public virtual Resident Requestee { get; set; } = null!;
        public virtual Resident Requester { get; set; } = null!;
        public virtual Schedule ScheduleSwap { get; set; } = null!;
    }
}
