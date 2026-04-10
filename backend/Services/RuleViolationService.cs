using MedicalDemo.Algorithms.OnCallScheduleGenerator;
using MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Interfaces;
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
    private readonly List<ICallShiftConstraint> _constraints;
    private readonly MedicalContext _context;
    private readonly SchedulerService _schedulerService;

    public RuleViolationService(
        MedicalContext context,
        ILogger<SchedulerService> logger,
        IEnumerable<ICallShiftConstraint> constraints,
        SchedulerService schedulerService
    )
    {
        _context = context;
        _logger = logger;
        _constraints = constraints.ToList();
        _schedulerService = schedulerService;
    }


    public async Task<(bool Success, string? Error, Resident? resident)> CheckResidentScheduledOnDate(
        Guid scheduleId,
        string residentId,
        DateOnly date
        )
    {
        //validate schedule and resident
        Schedule? schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            return (false, "Invalid ScheduleID.", null);
        }

        Resident? resident = await _context.Residents.FindAsync(residentId);
        if (resident == null)
        {
            return (false, "Invalid ResidentID.", null);
        }

        // check if shift exists for this resident on this day
        bool residentScheduled = await _context.Dates
            .AnyAsync(d =>
                d.ScheduleId == scheduleId &&
                d.ResidentId == residentId &&
                d.ShiftDate == date);

        if (residentScheduled)
        {
            return (false, $"Resident is already scheduled on this date. Resident: {residentId}, Schedule:{scheduleId},Date:{date}", resident);
        }

        return (true, null, resident);
    }

    public async Task<(bool Success, string Error, ViolationResult? violationResult)> EvaluateConstraints(Guid scheduleId, string residentId, DateOnly date, bool isDateOnlyUpdate = false, bool isResidentUpdate = false)
    {
        //validate schedule
        Schedule? schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            return (false, $"Invalid ScheduleID {scheduleId}.", null);
        }

        ResidentDto? residentInfo = await _schedulerService.LoadResidentData(date.AcademicYear, date.Semester, residentId, scheduleId);

        if (residentInfo == null)
        {
            return (false, $"Resident {residentId} not found.", null);
        }

        List<ConstraintResult> violations = [];
        // check through all constrains for any rule violations
        foreach (ICallShiftConstraint constraint in _constraints)
        {
            // if updating date for same resident, bypass OneShiftADayConstraint
            if (!constraint.IsApplicable(isDateOnlyUpdate, isResidentUpdate))
            {
                continue;
            }
            ConstraintResult result = constraint.Evaluate(residentInfo, date);
            if (result.IsViolated)
            {
                violations.Add(result);
            }
        }

        return (true, "", new ViolationResult(violations));
    }
}