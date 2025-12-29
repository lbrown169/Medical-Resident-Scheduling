using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models;

[Table("schedules")]
public class Schedules
{
    [Key][Column("schedule_id")] public Guid ScheduleId { get; set; }

    [Column("status")][MaxLength(45)] public string Status { get; set; }
    
    [Column("generated_year")] public int GeneratedYear { get; set; }

}