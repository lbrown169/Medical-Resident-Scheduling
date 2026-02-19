using MedicalDemo.Algorithm;
using MedicalDemo.Enums;
using MedicalDemo.Models;
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

    public async Task<(bool Success, string? Error)> GenerateScheduleForSemester(int year, Semester semester)
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

                bool loosely = attempt > 50;

                bool success;
                switch (semester)
                {
                    case Semester.Fall:
                        success =
                            _algorithmService.Training(year, residentData.PGY1s, residentData.PGY2s,
                                residentData.PGY3s) &&
                            _algorithmService.Part1(year, residentData.PGY1s, residentData.PGY2s, loosely);
                        break;
                    case Semester.Spring:
                        success = _algorithmService.Part2(year, residentData.PGY1s, residentData.PGY2s, loosely);
                        break;
                    default:
                        return (false, "Semester not recognized.");
                }

                if (!success)
                {
                    _logger.LogInformation(
                        "Attempt #{attempt}: Schedule generation failed logically.", attempt);
                    continue;
                }

                // Save schedule record
                Schedule schedule = new()
                { ScheduleId = Guid.NewGuid(), Status = ScheduleStatus.UnderReview, GeneratedYear = year, Semester = semester};
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

                _logger.LogInformation("Succeeded on attempt #{attempt}", attempt);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt #{attempt}: Exception encountered", attempt);
            }

            await Task.Delay(500); // short delay between retries
        }

        return (false,
            "Failed after to generate a viable schedule. Try again.");
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