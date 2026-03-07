namespace MedicalDemo.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public int AcademicYear => dateTime.Month >= 7 ? dateTime.Year : dateTime.Year - 1;
    }

    extension(DateOnly date)
    {
        public int AcademicYear => date.Month >= 7 ? date.Year : date.Year - 1;
    }
}