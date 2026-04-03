using System.Globalization;
using MedicalDemo.Converters;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Services;

public class SchedulingMapperService
{
    private readonly MedicalContext _context;
    private readonly RotationTypeConverter _rotationTypeConverter;

    public SchedulingMapperService(MedicalContext context, RotationTypeConverter rotationTypeConverter)
    {
        _context = context;
        _rotationTypeConverter = rotationTypeConverter;
    }

    public Pgy1Dto MapToPGY1DTO(Resident resident, IEnumerable<Rotation> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        Dictionary<PartOfDay, List<DateOnly>> days = BuildVacations(vacations);
        Pgy1Dto dto = new()
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            MorningVacationRequests = [.. days[PartOfDay.Morning]],
            AfternoonVacationRequests = [.. days[PartOfDay.Afternoon]],
            CommitedWorkDays = [.. committedDates],
            InTraining = resident.GraduateYr == 1,
            RolePerMonth = BuildHospitalRoles(rotations)
        };

        return dto;
    }

    public Pgy2Dto MapToPGY2DTO(Resident resident, IEnumerable<Rotation> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        Dictionary<PartOfDay, List<DateOnly>> days = BuildVacations(vacations);
        Pgy2Dto dto = new()
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            MorningVacationRequests = [.. days[PartOfDay.Morning]],
            AfternoonVacationRequests = [.. days[PartOfDay.Afternoon]],
            CommitedWorkDays = [.. committedDates],
            InTraining = resident.GraduateYr == 2,
            RolePerMonth = BuildHospitalRoles(rotations)
        };

        return dto;
    }

    public Pgy3Dto MapToPGY3DTO(Resident resident, List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        Dictionary<PartOfDay, List<DateOnly>> days = BuildVacations(vacations);
        return new Pgy3Dto
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            MorningVacationRequests = [.. days[PartOfDay.Morning]],
            AfternoonVacationRequests = [.. days[PartOfDay.Afternoon]],
            CommitedWorkDays = [.. committedDates]
        };
    }

    private Dictionary<PartOfDay, List<DateOnly>> BuildVacations(IEnumerable<Vacation> vacations)
    {
        Dictionary<PartOfDay, List<DateOnly>> days = new() {
            { PartOfDay.Morning, []},
            { PartOfDay.Afternoon, []},
        };

        foreach (Vacation vacation in vacations)
        {
            PartOfDay part = PartOfDay.FromDbChar(vacation.HalfDay);
            if (part.HasFlag(PartOfDay.Morning))
            {
                days[PartOfDay.Morning].Add(vacation.Date);
            }

            if (part.HasFlag(PartOfDay.Afternoon))
            {
                days[PartOfDay.Afternoon].Add(vacation.Date);
            }
        }

        return days;
    }

    private HospitalRole[] BuildHospitalRoles(IEnumerable<Rotation> rotations)
    {
        HospitalRole[] hospitalRoles = Enumerable.Repeat(HospitalRole.Unassigned, 12).ToArray();
        foreach (Rotation rotation in rotations)
        {
            RotationType type = rotation.RotationType;
            hospitalRoles[rotation.RotationMonthOfYear.ToAcademicIndex()] = _rotationTypeConverter.CreateHospitalRoleFromRotationType(type);
        }

        return hospitalRoles;
    }
}