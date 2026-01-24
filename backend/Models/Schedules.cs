using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models;

[Table("schedules")]
public class Schedules
{
    [Key][Column("schedule_id")] public Guid ScheduleId { get; set; }

    [Column("status")] public ScheduleStatus Status { get; set; }

    [Column("GeneratedYear")] public int GeneratedYear { get; set; }

}