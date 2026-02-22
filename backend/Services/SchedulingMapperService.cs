using System.Globalization;
using MedicalDemo.Algorithm;
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

    public PGY1DTO MapToPGY1DTO(Resident resident, IList<HospitalRole> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        PGY1DTO dto = new PGY1DTO
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

    public PGY2DTO MapToPGY2DTO(Resident resident, IList<HospitalRole> rotations,
        List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        PGY2DTO dto = new()
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

    public PGY3DTO MapToPGY3DTO(Resident resident, List<Vacation> vacations,
        List<Date> dates)
    {
        List<DateOnly> committedDates = dates
            .Where(d => d.ResidentId == resident.ResidentId)
            .Select(d => d.ShiftDate)
            .ToList();

        return new PGY3DTO
        {
            ResidentId = resident.ResidentId,
            Name = resident.FirstName + " " + resident.LastName,
            VacationRequests
                = [.. vacations.Select(v => v.Date)],
            CommitedWorkDays = [.. committedDates]
        };
    }
}