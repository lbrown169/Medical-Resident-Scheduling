namespace MedicalDemo.Models.DTO.Scheduling;

public class HospitalRole
{
    private HospitalRole(string name, bool doesShort, bool doesLong,
        bool flexShort, bool flexLong)
    {
        DoesShort = doesShort;
        DoesLong = doesLong;
        FlexShort = flexShort;
        FlexLong = flexLong;
    }

    public string name { get; set; }
    public bool DoesShort { get; }
    public bool DoesLong { get; }
    public bool FlexShort { get; }
    public bool FlexLong { get; }

    public static HospitalRole Inpatient =>
        new("Inpatient", true, true, false, false);

    public static HospitalRole Geriatric =>
        new("Geriatric", true, true, false, false);

    public static HospitalRole PHPandIOP =>
        new("PHPandIOP", true, true, false, false);

    public static HospitalRole PsychConsults =>
        new("PsychConsults", true, true, false, false);

    public static HospitalRole CommP => new("CommP", false, true, false, false);
    public static HospitalRole CAP => new("CAP", false, true, false, false);

    public static HospitalRole Addiction =>
        new("Addiction", false, true, false, false);

    public static HospitalRole Forensic =>
        new("Forensic", false, true, false, false);

    public static HospitalRole Float => new("Float", false, true, false, false);

    public static HospitalRole Neurology =>
        new("Neurology", false, true, true, false);

    public static HospitalRole IMOutpatient =>
        new("IMOutpatient", false, true, true, false);

    public static HospitalRole IMInpatient =>
        new("IMInpatient", false, false, false, true);

    public static HospitalRole NightFloat =>
        new("NightFloat", false, false, false, false);

    public static HospitalRole EmergencyMed =>
        new("EmergencyMed", false, false, false, true);

    public static HospitalRole
        random() // THIS IS PURELY FOR TESTING I NED TO REMOVE IT
    {
        int seed = (int)DateTime.Now.Ticks;
        Random rnd = new();
        int index = rnd.Next(0, 8);
        return index switch
        {
            0 => Inpatient,
            1 => Geriatric,
            2 => PHPandIOP,
            3 => PsychConsults,
            4 => CommP,
            5 => CAP,
            6 => Addiction,
            7 => Forensic,
            8 => Float,
            9 => Neurology,
            10 => IMOutpatient,
            11 => IMInpatient,
            12 => NightFloat,
            13 => EmergencyMed,
            _ => throw new ArgumentOutOfRangeException(
                                "Invalid index for HospitalRole random selection."),
        };
        return null;
    }
}