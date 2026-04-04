
using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum Pgy4ConstraintType
{
    [Display(Name = "Chief Constraint")]
    ChiefConstraint,

    [Display(Name = "Inpatient and Consult in January and July Constraint")]
    InpatientConsultInJanAndJuly,

    [Display(Name = "Minimum of 2 Consults and Inpatients Per Resident Constraint")]
    Min2ConsultsInpatients,

    [Display(Name = "Maximum of 1 IOP, Forensic, Community, Addiction Per Month Constraint")]
    OneIopForenCommAddictPerMonth,
}