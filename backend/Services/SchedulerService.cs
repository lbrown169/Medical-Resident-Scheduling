using MedicalDemo.Algorithm;
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
        if (year < DateTime.Now.AcademicYear)
        {
            return (false,
                $"Year must be the current year or later."
            );
        }

        // validate semester input
        if (!(semester == Semester.Spring || semester == Semester.Fall))
        {
            return (false, $"Invalid semester input, must be Fall or Spring.");
        }

        List<ResidentRequirementInfo> residentRequirementInfo = await _context.Residents
            .Select(r => new ResidentRequirementInfo
            {
                GraduateYr = r.GraduateYr,
                HasHospitalRoleProfile = r.HospitalRoleProfile.HasValue
            })
            .ToListAsync();


        int pgy1Count = residentRequirementInfo.Count(r => r.GraduateYr == 1);
        int pgy2Count = residentRequirementInfo.Count(r => r.GraduateYr == 2);
        int pgy3Count = residentRequirementInfo.Count(r => r.GraduateYr == 3);

        int pgy1HospitalRoleCount = residentRequirementInfo.Count(r => r.GraduateYr == 1 && r.HasHospitalRoleProfile);
        int pgy2HospitalRoleCount = residentRequirementInfo.Count(r => r.GraduateYr == 2 && r.HasHospitalRoleProfile);

        // 8 pgys exist
        bool hasRequiredPgy1 = pgy1Count >= 8;
        bool hasRequiredPgy2 = pgy2Count >= 8;
        bool hasRequiredPgy3 = pgy3Count >= 8;

        // all residents of PGY have hospital role profile assigned
        bool hasHospitalProfilePgy1 = pgy1Count == pgy1HospitalRoleCount;
        bool hasHospitalProfilePgy2 = pgy2Count == pgy2HospitalRoleCount;

        if (!hasRequiredPgy1)
        {
            return (false, $"Invalid Input - Cannot generate schedules: Missing {8 - pgy1Count} PGY-1(s). Minimum 8 PGY-1s required to generate schedule.");
        }

        if (!hasHospitalProfilePgy1)
        {
            return (false,
                $"Invalid Input - Cannot generate schedules: All PGY1s required to have hospital role assigned generate schedule, {pgy1Count - pgy1HospitalRoleCount} resident(s) missing hospital role."
            );
        }

        if (!hasRequiredPgy2)
        {
            return (false,
                    $"Invalid Input - Cannot generate schedules: Missing {8 - pgy2Count} PGY-2(s). Minimum 8 PGY-2s required to generate schedule."
            );
        }

        if (!hasHospitalProfilePgy2)
        {
            return (false,
                $"Invalid Input - Cannot generate schedules: All PGY2s required to have hospital role assigned generate schedule, {pgy2Count - pgy2HospitalRoleCount} resident(s) missing hospital role."
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
                $"Invalid Input - Cannot generate schedules: Missing {8 - pgy3Count} PGY-3(s). Minimum 8 PGY-3s required to generate Fall Schedule."
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
                ResidentData residentData = await LoadResidentData(year);

                // Map DTOs to original algorithm classes
                // List<PGY1> pgy1Models = residentData.PGY1s
                //     .Select(MapToPGY1).ToList();
                // List<PGY2> pgy2Models = residentData.PGY2s
                //     .Select(MapToPGY2).ToList();
                // List<PGY3> pgy3Models = residentData.PGY3s
                //     .Select(MapToPGY3).ToList();

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
                { ScheduleId = Guid.NewGuid(), Status = ScheduleStatus.UnderReview, GeneratedYear = year, Semester = semester };
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();

                // Generate DatesDTOs from PGY models
                List<DatesDTO> dateDTOs =
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
                    GeneratedYear = schedule.GeneratedYear,
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

#pragma warning disable IDE0060
    private async Task<ResidentData> LoadResidentData(int year)
    {
        List<Resident> residents = await _context.Residents.ToListAsync();
        List<Rotation> rotations = await _context.Rotations.ToListAsync();
        List<Vacation> vacations = await _context.Vacations
            .Where(v => v.Status == "Approved").ToListAsync();
        List<Date> dates = await _context.Dates.Where(d => d.Schedule.Status == ScheduleStatus.Published && d.Schedule.GeneratedYear == year).ToListAsync();

        List<PGY1DTO> pgy1s = residents
            .Where(r => r.GraduateYr == 1)
            .Select(r => _mapper.MapToPGY1DTO(
                r,
                r.HospitalRoleProfile is { } role ? HospitalRole.Pgy1Profiles[role] : [],
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        List<PGY2DTO> pgy2s = residents
            .Where(r => r.GraduateYr == 2)
            .Select(r => _mapper.MapToPGY2DTO(r,
                r.HospitalRoleProfile is { } role ? HospitalRole.Pgy2Profiles[role - 8] : [],
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        List<PGY3DTO> pgy3s = residents
            .Where(r => r.GraduateYr == 3)
            .Select(r => _mapper.MapToPGY3DTO(r,
                vacations.Where(v => v.ResidentId == r.ResidentId).ToList(),
                dates)).ToList();

        return new ResidentData { PGY1s = pgy1s, PGY2s = pgy2s, PGY3s = pgy3s };
    }
#pragma warning restore IDE0060
}