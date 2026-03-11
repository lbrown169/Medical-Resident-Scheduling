using MedicalDemo.Enums;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Extensions;

public static class ResidentExtensions
{
    public static int GetGraduateYrForDate(this Resident resident, DateOnly date)
    {
        int graduateYr = GetGraduateYrForSemesterAndYear(resident, date.Semester, date.Year);
        return graduateYr;
    }

    public static int GetGraduateYrForSemesterAndYear(this Resident resident, Semester semester, int year)
    {
        int offset = 0;
        int currAcademicYear = DateTime.Now.AcademicYear;
        int inputtedDateAcademicYear = semester == Semester.Fall ? year : year - 1;

        // calc how many academic years ahead of today the manual date is
        if (inputtedDateAcademicYear > currAcademicYear)
        {
            offset = inputtedDateAcademicYear - currAcademicYear;
        }

        int graduateYr = resident.GraduateYr + offset;
        return (graduateYr);
    }
}