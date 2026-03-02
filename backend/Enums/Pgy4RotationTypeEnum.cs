using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum Pgy4RotationTypeEnum
{
    [Display(Name = "Inpatient Psy")]
    InpatientPsy,

    [Display(Name = "Forensic")]
    Forensic,

    [Display(Name = "Community Psy")]
    CommunityPsy,

    [Display(Name = "Addiction")]
    Addiction,

    [Display(Name = "Psy Consults")]
    PsyConsults,

    [Display(Name = "TMS")]
    TMS,

    [Display(Name = "CLC")]
    CLC,

    [Display(Name = "NFETC")]
    NFETC,

    [Display(Name = "HPC")]
    HPC,

    [Display(Name = "Chief")]
    Chief,

    [Display(Name = "Sum")]
    Sum,

    [Display(Name = "IOP")]
    IOP,

    [Display(Name = "VA")]
    VA,
}