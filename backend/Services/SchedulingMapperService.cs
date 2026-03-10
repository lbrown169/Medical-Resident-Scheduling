using System.Globalization;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Services;

public class SchedulingMapperService
{
    private readonly MedicalContext _context;

    public SchedulingMapperService(MedicalContext context)
    {
        _context = context;
    }

    public Pgy1Dto MapToPGY1DTO(Resident resident, IList<HospitalRole> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        Pgy1Dto dto = new Pgy1Dto
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            VacationRequests
                = [.. vacations.Select(v => v.Date)],
            CommitedWorkDays = [.. committedDates],
            InTraining = resident.GraduateYr == 1
        };

        for (int i = 0; i < rotations.Count; i++)
        {
            dto.RolePerMonth[i] = rotations[i];
        }

        return dto;
    }

    public Pgy2Dto MapToPGY2DTO(Resident resident, IList<HospitalRole> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        Pgy2Dto dto = new()
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            VacationRequests
                = [.. vacations.Select(v => v.Date)],
            CommitedWorkDays = [.. committedDates],
            InTraining = resident.GraduateYr == 2
        };

        for (int i = 0; i < rotations.Count; i++)
        {
            dto.RolePerMonth[i] = rotations[i];
        }

        return dto;
    }

    public Pgy3Dto MapToPGY3DTO(Resident resident, List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        return new Pgy3Dto
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            VacationRequests
                = [.. vacations.Select(v => v.Date)],
            CommitedWorkDays = [.. committedDates]
        };
    }
}