namespace MedicalDemo.Models.DTO.Scheduling;

public class HospitalRole
{
#pragma warning disable IDE0060
    private HospitalRole(string name, bool doesShort, bool doesLong,
        bool doesTrainingShort, bool doesTrainingLong)
    {
        DoesShort = doesShort;
        DoesLong = doesLong;
        DoesTrainingShort = doesTrainingShort;
        DoesTrainingLong = doesTrainingLong;
    }
#pragma warning restore IDE0060

    public string name { get; set; }
    public bool DoesShort { get; }
    public bool DoesLong { get; }
    public bool DoesTrainingShort { get; }
    public bool DoesTrainingLong { get; }

    public static HospitalRole Unassigned =>
        new("Unassigned", true, true, true, true);

    public static HospitalRole Inpatient =>
        new("Inpatient", true, true, true, true);

    public static HospitalRole Geriatric =>
        new("Geriatric", true, true, true, true);

    public static HospitalRole PHPandIOP =>
        new("PHPandIOP", true, true, true, true);

    public static HospitalRole PsychConsults =>
        new("PsychConsults", true, true, true, true);

    public static HospitalRole CommP => new("CommP", true, true, true, true);
    public static HospitalRole CAP => new("CAP", true, true, true, true);

    public static HospitalRole Addiction =>
        new("Addiction", true, true, true, true);

    public static HospitalRole Forensic =>
        new("Forensic", true, true, true, true);

    public static HospitalRole Float => new("Float", true, true, true, true);

    public static HospitalRole Neurology =>
        new("Neurology", false, true, true, true);

    public static HospitalRole IMOutpatient =>
        new("IMOutpatient", false, true, true, true);

    public static HospitalRole IMInpatient =>
        new("IMInpatient", false, false, false, false);

    public static HospitalRole NightFloat =>
        new("NightFloat", true, true, true, true);

    public static HospitalRole EmergencyMed =>
        new("EmergencyMed", false, false, false, false);

    public static readonly HospitalRole[] Pgy1Profile1 =
    [
        PsychConsults,
        Inpatient,
        Inpatient,
        Inpatient,
        Inpatient,
        NightFloat,
        IMOutpatient,
        Neurology,
        EmergencyMed,
        IMOutpatient,
        IMInpatient,
        Neurology
    ];

    public static readonly HospitalRole[] Pgy1Profile2 =
    [
        Inpatient,
        Inpatient,
        Inpatient,
        PsychConsults,
        NightFloat,
        Inpatient,
        EmergencyMed,
        IMOutpatient,
        Neurology,
        IMInpatient,
        IMOutpatient,
        Neurology
    ];

    public static readonly HospitalRole[] Pgy1Profile3 =
    [
        Inpatient,
        Inpatient,
        PsychConsults,
        NightFloat,
        Inpatient,
        Inpatient,
        Neurology,
        IMInpatient,
        IMOutpatient,
        Neurology,
        EmergencyMed,
        IMOutpatient
    ];

    public static readonly HospitalRole[] Pgy1Profile4 =
    [
        Inpatient,
        PsychConsults,
        NightFloat,
        Inpatient,
        Inpatient,
        Inpatient,
        Neurology,
        IMOutpatient,
        IMInpatient,
        IMOutpatient,
        Neurology,
        EmergencyMed
    ];

    public static readonly HospitalRole[] Pgy1Profile5 =
    [
        Neurology,
        EmergencyMed,
        IMOutpatient,
        IMInpatient,
        Neurology,
        IMOutpatient,
        PsychConsults,
        Inpatient,
        NightFloat,
        Inpatient,
        Inpatient,
        Inpatient
    ];

    public static readonly HospitalRole[] Pgy1Profile6 =
    [
        Neurology,
        IMOutpatient,
        EmergencyMed,
        IMOutpatient,
        IMInpatient,
        Neurology,
        Inpatient,
        PsychConsults,
        Inpatient,
        NightFloat,
        Inpatient,
        Inpatient,
    ];

    public static readonly HospitalRole[] Pgy1Profile7 =
    [
        IMOutpatient,
        IMInpatient,
        Neurology,
        Neurology,
        EmergencyMed,
        IMOutpatient,
        Inpatient,
        Inpatient,
        PsychConsults,
        Inpatient,
        NightFloat,
        Inpatient,
    ];

    public static readonly HospitalRole[] Pgy1Profile8 =
    [
        EmergencyMed,
        Neurology,
        IMInpatient,
        IMOutpatient,
        IMOutpatient,
        Neurology,
        Inpatient,
        Inpatient,
        Inpatient,
        PsychConsults,
        Inpatient,
        NightFloat,
    ];

    public static readonly HospitalRole[][] Pgy1Profiles = [Pgy1Profile1, Pgy1Profile2, Pgy1Profile3, Pgy1Profile4, Pgy1Profile5, Pgy1Profile6, Pgy1Profile7, Pgy1Profile8];

    public static readonly HospitalRole[] Pgy2Profile1 =
    [
        CAP,
        CAP,
        CommP,
        Forensic,
        Float,
        Addiction,
        Inpatient,
        NightFloat,
        PHPandIOP,
        PsychConsults,
        Geriatric,
        Inpatient
    ];

    public static readonly HospitalRole[] Pgy2Profile2 =
    [
        Addiction,
        CAP,
        CAP,
        Float,
        Forensic,
        CommP,
        NightFloat,
        Inpatient,
        PsychConsults,
        PHPandIOP,
        Inpatient,
        Geriatric,
    ];

    public static readonly HospitalRole[] Pgy2Profile3 =
    [
        Forensic,
        Addiction,
        CAP,
        CAP,
        CommP,
        Float,
        Geriatric,
        NightFloat,
        Inpatient,
        Inpatient,
        PHPandIOP,
        PsychConsults
    ];

    public static readonly HospitalRole[] Pgy2Profile4 =
    [
        CommP,
        Forensic,
        Float,
        Addiction,
        CAP,
        CAP,
        NightFloat,
        Geriatric,
        Inpatient,
        Inpatient,
        PsychConsults,
        PHPandIOP,
    ];

    public static readonly HospitalRole[] Pgy2Profile5 =
    [
        NightFloat,
        Geriatric,
        PHPandIOP,
        Inpatient,
        Inpatient,
        PsychConsults,
        CAP,
        CAP,
        Addiction,
        Float,
        Forensic,
        CommP
    ];

    public static readonly HospitalRole[] Pgy2Profile6 =
    [
        NightFloat,
        Inpatient,
        Inpatient,
        PsychConsults,
        PHPandIOP,
        Geriatric,
        Forensic,
        CommP,
        Float,
        Addiction,
        CAP,
        CAP
    ];

    public static readonly HospitalRole[] Pgy2Profile7 =
    [
        Inpatient,
        NightFloat,
        PsychConsults,
        PHPandIOP,
        Geriatric,
        Inpatient,
        CommP,
        Addiction,
        Forensic,
        CAP,
        CAP,
        Float
    ];

    public static readonly HospitalRole[] Pgy2Profile8 =
    [
        Geriatric,
        NightFloat,
        Inpatient,
        Inpatient,
        PsychConsults,
        PHPandIOP,
        Addiction,
        CAP,
        CAP,
        CommP,
        Float,
        Forensic
    ];

    public static readonly HospitalRole[][] Pgy2Profiles = [Pgy2Profile1, Pgy2Profile2, Pgy2Profile3, Pgy2Profile4, Pgy2Profile5, Pgy2Profile6, Pgy2Profile7, Pgy2Profile8];
}