using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class RuleViolationService
{
    private readonly ILogger<SchedulerService> _logger;
    private readonly AlgorithmService _algorithmService;
    private readonly MedicalContext _context;
    private readonly SchedulingMapperService _mapper;

    public RuleViolationService(
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


    public async Task<(bool Success, string? Error)> CheckResidentScheduledOnDate(
        Guid schedule_id,
        string resident_id,
        DateOnly date
        )
    {
        //validate schedule and resident
        Schedule? schedule = await _context.Schedules.FindAsync(schedule_id);
        if (schedule == null)
        {
            return (false, "Invalid ScheduleID.");
        }

        Resident? resident = await _context.Residents.FindAsync(resident_id);
        if (resident == null)
        {
            return (false, "Invalid ResidentID.");
        }

        // check if shift exists for this resident on this day
        bool residentScheduled = await _context.Dates
            .AnyAsync(d =>
                d.ResidentId == resident_id &&
                d.ShiftDate == date);

        if (residentScheduled)
        {
            return (false, "Resident is already scheduled on this date.");
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error, int offset)> CalcPGYearOffset(
        Guid schedule_id, string resident_id, DateOnly date)
    {
        //validate schedule and resident
        Schedule? schedule = await _context.Schedules.FindAsync(schedule_id);
        if (schedule == null)
        {
            return (false, "Invalid ScheduleID.", -1);
        }

        Resident? resident = await _context.Residents.FindAsync(resident_id);
        if (resident == null)
        {
            return (false, "Invalid ResidentID.", -1);
        }

        // calc how many academic years ahead of today the manual date is
        int offset = 0;
        if (date.AcademicYear > DateTime.Now.AcademicYear)
        {
            offset = date.AcademicYear - DateTime.Now.AcademicYear;
        }

        return (true, null, offset);

    }

    public async Task<(bool Success, string? Error, int graduateYr)> CalcGradYearWOffset(string resident_id, int offset)
    {
        Resident? resident = await _context.Residents.FindAsync(resident_id);
        if (resident == null)
        {
            return (false, "Invalid ResidentID.", -1);
        }

        int graduateYr = resident.GraduateYr;

        graduateYr += offset;

        return (true, null, graduateYr);

    }
}