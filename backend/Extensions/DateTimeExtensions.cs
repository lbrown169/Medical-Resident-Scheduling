namespace MedicalDemo.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public int AcademicYear => dateTime.Month >= 7 ? dateTime.Year : dateTime.Year - 1;
    }
}