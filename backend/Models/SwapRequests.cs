using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Data.Models
{
    [Table("swap_requests")]
    public class SwapRequest
    {
        [Key]
        [Column("idswap_requests")]
        public Guid SwapId { get; set; }

        [Required]
        [Column("schedule_swap_id")]
        public Guid ScheduleSwapId { get; set; }

        [Required]
        [Column("requester_id")]
        [MaxLength(15)]
        public string RequesterId { get; set; }

        [Required]
        [Column("requestee_id")]
        [MaxLength(15)]
        public string RequesteeId { get; set; }

        [Required]
        [Column("requester_date")]
        public DateTime RequesterDate { get; set; }

        [Required]
        [Column("requestee_date")]
        public DateTime RequesteeDate { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(45)]
        public string Status { get; set; } = "Pending";

        [Required]
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at", TypeName = "timestamp")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        [Column("details")]
        [MaxLength(150)]
        public string? Details { get; set; }
    }
}