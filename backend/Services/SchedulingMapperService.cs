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

    public List<DatesDTO> MapToDatesDTOs(List<Date> dates)
    {
        return dates.Select(d => new DatesDTO
        {
            DateId = d.DateId,
            ScheduleId = d.ScheduleId,
            ResidentId = d.ResidentId,
            Date = d.ShiftDate,
            CallType = d.CallType,
            IsCommitted = true
        }).ToList();
    }

    private static HospitalRole[] MapRotationsToRoles(List<Rotation> rotations)
    {
        HospitalRole[] roles = new HospitalRole[12];

        foreach (Rotation rotation in rotations)
        {
            _ = rotation.Rotation1?.Trim().ToLowerInvariant();
            int month = DateOnly.ParseExact(rotation.Month.Trim(), "MMMM",
                CultureInfo.InvariantCulture).Month;
            int academicMonthIndex = (month + 5) % 12;
            roles[academicMonthIndex]
                = MapRotationNameToRole(rotation.Rotation1);
        }

        return roles;
    }

    private static HospitalRole MapRotationNameToRole(string rotationName)
    {
        if (string.IsNullOrWhiteSpace(rotationName))
        {
            throw new ArgumentException("Rotation name is null or empty.");
        }

        string key = rotationName.Trim().ToLowerInvariant();

        return key switch
        {
            "inpt psy" => HospitalRole.Inpatient,
            "geri" => HospitalRole.Geriatric,
            "php/iop" => HospitalRole.PHPandIOP,
            "consult" => HospitalRole.PsychConsults,
            "addiction" => HospitalRole.Addiction,
            "forensic" => HospitalRole.Forensic,
            "float" => HospitalRole.Float,
            "neuro" => HospitalRole.Neurology,
            "imop" => HospitalRole.IMOutpatient,
            "imip" => HospitalRole.IMInpatient,
            "night float" => HospitalRole.NightFloat,
            "emergency med" => HospitalRole.EmergencyMed,
            "er-p" => HospitalRole.NightFloat,
            "er p/cl" => HospitalRole.NightFloat,
            "child" => HospitalRole.CAP,
            "comm" => HospitalRole.CommP,
            "cl/er p" => HospitalRole.NightFloat,
            "em" => HospitalRole.EmergencyMed,
            _ => throw new ArgumentException("Unknown rotation: '" +
                                             rotationName + "'")
        };
    }

    // === Mapping to Algorithm Classes ===
    public List<PGY1> MapToPGY1AlgoModels(List<PGY1DTO> dtos)
    {
        return dtos.Select(dto =>
        {
            PGY1 model = new(dto.Name)
            {
                inTraining = dto.InTraining,
                lastTrainingDate = dto.LastTrainingDate
            };
            for (int i = 0; i < 12; i++)
            {
                model.rolePerMonth[i] = dto.RolePerMonth[i];
            }

            foreach (DateOnly day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateOnly day in dto.CommitedWorkDays)
            {
                model.addWorkDay(day);
            }

            return model;
        }).ToList();
    }

    public List<PGY2> MapToPGY2AlgoModels(List<PGY2DTO> dtos)
    {
        return dtos.Select(dto =>
        {
            PGY2 model = new(dto.Name)
            {
                inTraining = dto.InTraining
            };
            for (int i = 0; i < 12; i++)
            {
                model.rolePerMonth[i] = dto.RolePerMonth[i];
            }

            foreach (DateOnly day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateOnly day in dto.CommitedWorkDays)
            {
                model.addWorkDay(day);
            }

            return model;
        }).ToList();
    }

    public List<PGY3> MapToPGY3AlgoModels(List<PGY3DTO> dtos)
    {
        return dtos.Select(dto =>
        {
            PGY3 model = new(dto.Name);
            foreach (DateOnly day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateOnly day in dto.CommitedWorkDays)
            {
                model.addWorkDay(day);
            }

            return model;
        }).ToList();
    }
}