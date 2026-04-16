using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class SchedulerService
{
    private readonly ILogger<SchedulerService> _logger;
    private readonly AlgorithmService _algorithmService;
    private readonly MedicalContext _context;
    private readonly SchedulingMapperService _mapper;

    public SchedulerService(
        MedicalContext context,
        SchedulingMapperService mapper,
        AlgorithmService algorithmService,
        ILogger<SchedulerService> logger
    )
    {
        _context = context;
        _mapper = mapper;
        _algorithmService = algorithmService;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> CheckScheduleRequirements(int year,
        Semester semester)
    {
        // validate year input
        int academicYear = semester == Semester.Fall ? year : year - 1;
        if (academicYear < DateTime.Now.AcademicYear)
        {
            return (false,
                $"Year must be the current academic year or later."
            );
        }

        // validate semester input
        if (!(semester == Semester.Spring || semester == Semester.Fall))
        {
            return (false, $"Invalid semester input, must be Fall or Spring.");
        }

        List<ResidentRequirementInfo> residentRequirementInfo = await _context.Residents
            .Where(r => r.GraduateYr != null)
            .Select(r => new ResidentRequirementInfo
            {
                GraduateYr = r.GraduateYr!.Value,
                ResidentId = r.ResidentId
            })
            .ToListAsync();

        int pgyDiff = academicYear - DateTime.Now.AcademicYear;

        List<Rotation> rotations = await _context.Rotations
            .Where(r => r.AcademicYear == academicYear && r.ResidentId != null)
            .ToListAsync();

        int pgy1Count = residentRequirementInfo.Count(r => r.GraduateYr + pgyDiff == 1);
        int pgy2Count = residentRequirementInfo.Count(r => r.GraduateYr + pgyDiff == 2);
        int pgy3Count = residentRequirementInfo.Count(r => r.GraduateYr + pgyDiff == 3);

        int pgy1HospitalRoleCount = residentRequirementInfo.Count(r => r.GraduateYr + pgyDiff == 1 && rotations.Count(rot => rot.ResidentId == r.ResidentId) == 12);
        int pgy2HospitalRoleCount = residentRequirementInfo.Count(r => r.GraduateYr + pgyDiff == 2 && rotations.Count(rot => rot.ResidentId == r.ResidentId) == 12);

        // 8 pgys exist
        bool hasRequiredPgy1 = pgy1Count >= 8;
        bool hasRequiredPgy2 = pgy2Count >= 8;
        bool hasRequiredPgy3 = pgy3Count >= 8;

        // all residents of PGY have hospital role profile assigned
        bool hasHospitalProfilePgy1 = pgy1Count == pgy1HospitalRoleCount;
        bool hasHospitalProfilePgy2 = pgy2Count == pgy2HospitalRoleCount;

        if (!hasRequiredPgy1)
        {
            return (false, $"Error generating schedules: Missing {8 - pgy1Count} PGY-1(s). Minimum 8 PGY-1s required.");
        }

        if (!hasHospitalProfilePgy1)
        {
            return (false,
                $"Error generating schedules: {pgy1Count - pgy1HospitalRoleCount} PGY-1 resident(s) not assigned rotation(s)."
            );
        }

        if (!hasRequiredPgy2)
        {
            return (false,
                    $"Error generating schedules: Missing {8 - pgy2Count} PGY-2(s). Minimum 8 PGY-2s required."
            );
        }

        if (!hasHospitalProfilePgy2)
        {
            return (false,
                $"Error generating schedules: {pgy2Count - pgy2HospitalRoleCount} PGY-2 resident(s) not assigned rotation(s)."
            );
        }

        if (semester == Semester.Spring)
        {
            return (true,
                $"Spring Schedule Generation Requirements Passed."
            );
        }

        // fall semester
        if (hasRequiredPgy3)
        {
            return (true,
                $"Fall Schedule Generation Requirements Passed."
            );
        }

        return (false,
                $"Error generating schedules: Missing {8 - pgy3Count} PGY-3(s). Minimum 8 PGY-3s required."
        );
    }

    public async Task<(bool Success, string? Error, ScheduleResponse? Schedule)> GenerateScheduleForSemester(int year, Semester semester)
    {
        const int maxRetries = 100;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            attempt++;
            try
            {
                ResidentData residentData = await LoadAllResidentData(year, semester);

                int looseFactor = 6 + Math.Min(18, attempt / 10 * 2);

                bool success;
                switch (semester)
                {
                    case Semester.Fall:
                        success =
                            _algorithmService.Training(year, residentData.PGY1s, residentData.PGY2s,
                                residentData.PGY3s) &&
                            _algorithmService.Part1(year, residentData.PGY1s, residentData.PGY2s, looseFactor);
                        break;
                    case Semester.Spring:
                        success = _algorithmService.Part2(year, residentData.PGY1s, residentData.PGY2s, looseFactor);
                        break;
                    default:
                        return (false, "Semester not recognized.", null);
                }

                if (!success)
                {
                    _logger.LogInformation(
                        "Attempt #{attempt}: Schedule generation failed logically.", attempt);
                    continue;
                }

                // Save schedule record
                Schedule schedule = new()
                { ScheduleId = Guid.NewGuid(), Status = ScheduleStatus.UnderReview, Year = year, Semester = semester };
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();

                // Generate DatesDTOs from PGY models
                List<DatesDto> dateDTOs =
                    AlgorithmService.GenerateDateRecords(schedule.ScheduleId, residentData.PGY1s,
                        residentData.PGY2s,
                        residentData.PGY3s);

                // Convert DTOs to Entities
                List<Date> dateEntities = dateDTOs.Select(dto => new Date
                {
                    DateId = dto.DateId,
                    ScheduleId = dto.ScheduleId,
                    ResidentId = dto.ResidentId,
                    ShiftDate = dto.Date,
                    Hours = dto.Hours,
                    CallType = dto.CallType
                }).ToList();

                // Then load the new schedule into the database
                await _context.Dates.AddRangeAsync(dateEntities);
                await _context.SaveChangesAsync();

                //add the total and bi-yearly hours for us after the fact lmao
                // await _misc.FindTotalHours();
                // await _misc.FindBiYearlyHours(year);

                // Calc residents hours for ScheduleResponse in AlgorithmController
                Dictionary<string, int> residentHours = dateEntities
                    .GroupBy(d => d.ResidentId)
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.Hours));

                ScheduleResponse scheduleResponse = new ScheduleResponse()
                {
                    ScheduleId = schedule.ScheduleId,
                    Status = new ScheduleStatusResponse(schedule.Status),
                    Year = schedule.Year,
                    Semester = new SemesterInfoResponse(schedule.Semester),
                    ResidentHours = residentHours,
                    TotalHours = residentHours.Values.Sum(),
                    TotalResidents = residentHours.Count
                };


                _logger.LogInformation("Succeeded on attempt #{attempt}", attempt);
                return (true, null, scheduleResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt #{attempt}: Exception encountered", attempt);
            }

            await Task.Delay(500); // short delay between retries
        }

        return (false,
            "Failed after to generate a viable schedule. Try again.", null);
    }

    public async Task<ResidentData> LoadAllResidentData(int year, Semester semester, IEnumerable<Guid>? existingSchedule = null)
    {
        int academicYear = semester == Semester.Fall ? year : year - 1;
        int pgyDiff = academicYear - DateTime.Now.AcademicYear;

        List<Resident> residents = await _context.Residents.ToListAsync();
        List<Rotation> rotations = await _context.Rotations
            .Include(r => r.RotationType)
            .Where(r => r.AcademicYear == academicYear && r.ResidentId != null)
            .ToListAsync();
        List<Vacation> vacations = await _context.Vacations
            .Where(v => v.Status == "Approved").ToListAsync();

        List<Date> dates = [];

        if (existingSchedule != null)
        {
            IQueryable<Guid> scheduleIds = existingSchedule.AsQueryable();
            dates = await _context.Dates
                .Where(d =>
                    scheduleIds.Contains(d.Schedule.ScheduleId)
                )
                .ToListAsync();
        }

        List<Pgy1Dto> pgy1s = residents
            .Where(r => r.GraduateYr + pgyDiff == 1)
            .Select(r => _mapper.MapToPgy1Dto(
                r,
                rotations.Where(rot => rot.ResidentId == r.ResidentId),
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        List<Pgy2Dto> pgy2s = residents
            .Where(r => r.GraduateYr + pgyDiff == 2)
            .Select(r => _mapper.MapToPgy2Dto(
                r,
                rotations.Where(rot => rot.ResidentId == r.ResidentId),
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        List<Pgy3Dto> pgy3s = residents
            .Where(r => r.GraduateYr + pgyDiff == 3)
            .Select(r => _mapper.MapToPgy3Dto(r,
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        return new ResidentData { PGY1s = pgy1s, PGY2s = pgy2s, PGY3s = pgy3s };
    }

    public async Task<ResidentDto?> LoadResidentData(int year, Semester semester, string residentId, Guid scheduleId)
    {
        int academicYear = semester == Semester.Fall ? year : year - 1;
        int pgyDiff = academicYear - DateTime.Now.AcademicYear;

        Resident? resident = await _context.Residents.FindAsync(residentId);

        if (resident == null)
        {
            return null;
        }

        List<Rotation> rotations = await _context.Rotations
            .Include(r => r.RotationType)
            .Where(r => r.AcademicYear == academicYear && r.ResidentId == residentId)
            .ToListAsync();
        List<Vacation> vacations = await _context.Vacations
            .Where(v => v.Status == "Approved").ToListAsync();

        List<Date> dates = await _context.Dates
            .Where(d => d.ResidentId == residentId && d.Schedule.ScheduleId == scheduleId
            )
            .ToListAsync();

        return (resident.GraduateYr + pgyDiff) switch
        {
            1 => _mapper.MapToPgy1Dto(resident,
                rotations.Where(rot => rot.ResidentId == resident.ResidentId),
                vacations.Where(v => v.ResidentId == resident.ResidentId).ToList(), dates),
            2 => _mapper.MapToPgy2Dto(resident,
                rotations.Where(rot => rot.ResidentId == resident.ResidentId),
                vacations.Where(v => v.ResidentId == resident.ResidentId).ToList(), dates),
            3 => _mapper.MapToPgy3Dto(resident,
                vacations.Where(v => v.ResidentId == resident.ResidentId).ToList(), dates),
            _ => null
        };
    }
}