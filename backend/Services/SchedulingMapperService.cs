using System.Globalization;
using MedicalDemo.Algorithm;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalDemo.Services;

public class SchedulingMapperService
{
    private readonly MedicalContext _context;

    public SchedulingMapperService(MedicalContext context)
    {
        _context = context;
    }

    public PGY1DTO MapToPGY1DTO(Residents resident, List<Rotations> rotations,
        List<Vacations> vacations,
        List<DatesDTO> dates)
    {
        List<DateTime> committedDates = dates
            .Where(d => d.ResidentId == resident.resident_id && d.IsCommitted)
            .Select(d => d.Date)
            .ToList();

        return new PGY1DTO
        {
            ResidentId = resident.resident_id,
            Name = resident.first_name + " " + resident.last_name,
            VacationRequests
                = new HashSet<DateTime>(vacations.Select(v => v.Date)),
            RolePerMonth = MapRotationsToRoles(rotations),
            CommitedWorkDays = new HashSet<DateTime>(committedDates),
            InTraining = resident.graduate_yr == 1
        };
    }

    public PGY2DTO MapToPGY2DTO(Residents resident, List<Rotations> rotations,
        List<Vacations> vacations,
        List<DatesDTO> dates)
    {
        List<DateTime> committedDates = dates
            .Where(d => d.ResidentId == resident.resident_id && d.IsCommitted)
            .Select(d => d.Date)
            .ToList();

        return new PGY2DTO
        {
            ResidentId = resident.resident_id,
            Name = resident.first_name + " " + resident.last_name,
            VacationRequests
                = new HashSet<DateTime>(vacations.Select(v => v.Date)),
            RolePerMonth = MapRotationsToRoles(rotations),
            CommitedWorkDays = new HashSet<DateTime>(committedDates),
            InTraining = resident.graduate_yr == 2
        };
    }

    public PGY3DTO MapToPGY3DTO(Residents resident, List<Vacations> vacations,
        List<DatesDTO> dates)
    {
        List<DateTime> committedDates = dates
            .Where(d => d.ResidentId == resident.resident_id && d.IsCommitted)
            .Select(d => d.Date)
            .ToList();

        return new PGY3DTO
        {
            ResidentId = resident.resident_id,
            Name = resident.first_name + " " + resident.last_name,
            VacationRequests
                = new HashSet<DateTime>(vacations.Select(v => v.Date)),
            CommitedWorkDays = new HashSet<DateTime>(committedDates)
        };
    }

    public List<DatesDTO> MapToDatesDTOs(List<Dates> dates)
    {
        return dates.Select(d => new DatesDTO
        {
            DateId = d.DateId,
            ScheduleId = d.ScheduleId,
            ResidentId = d.ResidentId,
            Date = d.Date,
            CallType = d.CallType,
            IsCommitted = true
        }).ToList();
    }

    private static HospitalRole[] MapRotationsToRoles(List<Rotations> rotations)
    {
        HospitalRole[] roles = new HospitalRole[12];

        foreach (Rotations rotation in rotations)
        {
            _ = rotation.Rotation?.Trim().ToLowerInvariant();
            int month = DateTime.ParseExact(rotation.Month.Trim(), "MMMM",
                CultureInfo.InvariantCulture).Month;
            int academicMonthIndex = (month + 5) % 12;
            roles[academicMonthIndex]
                = MapRotationNameToRole(rotation.Rotation);
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

            foreach (DateTime day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateTime day in dto.CommitedWorkDays)
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

            foreach (DateTime day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateTime day in dto.CommitedWorkDays)
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
            foreach (DateTime day in dto.VacationRequests)
            {
                model.requestVacation(day);
            }

            foreach (DateTime day in dto.CommitedWorkDays)
            {
                model.addWorkDay(day);
            }

            return model;
        }).ToList();
    }
}