using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums
{
    public enum Semester
    {
        [Display(Name = "Fall")]
        Fall = 1,

        [Display(Name = "Spring")]
        Spring = 2
    }
}