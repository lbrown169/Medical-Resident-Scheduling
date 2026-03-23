using MedicalDemo.Enums;

namespace MedicalDemo.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime dateTime)
    {
        public int AcademicYear => DateOnly.FromDateTime(dateTime).AcademicYear;
        public Semester Semester => DateOnly.FromDateTime(dateTime).Semester;
    }

    extension(DateOnly date)
    {
        public int AcademicYear => date.Month >= 7 ? date.Year : date.Year - 1;
        public Semester Semester => date.Month >= 7 ? Semester.Fall : Semester.Spring;
    }
}