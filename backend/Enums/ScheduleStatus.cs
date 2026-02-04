using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum ScheduleStatus
{
    [Display(Name = "Under Review")]
    UnderReview = 0,
    [Display(Name = "Published")]
    Published = 1
}