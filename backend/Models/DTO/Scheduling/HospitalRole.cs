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

    public static HospitalRole[] Profile1 =
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

    public static HospitalRole[] Profile2 =
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

    public static HospitalRole[] Profile3 =
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

    public static HospitalRole[] Profile4 =
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

    public static HospitalRole[] Profile5 =
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

    public static HospitalRole[] Profile6 =
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

    public static HospitalRole[] Profile7 =
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

    public static HospitalRole[] Profile8 =
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

    public static HospitalRole[] GeneralProfile =
    [
        Inpatient, Inpatient, Inpatient, Inpatient, Inpatient, Inpatient,
        Inpatient, Inpatient, Inpatient, Inpatient, Inpatient, Inpatient,
    ];
    public static HospitalRole[][] Profiles = [Profile1, Profile2, Profile3, Profile4, Profile5, Profile6, Profile7, Profile8];
}