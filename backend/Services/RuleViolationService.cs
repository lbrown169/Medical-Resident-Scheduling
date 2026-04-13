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

    public async Task<ViolationResult> EvaluateConstraints(Guid scheduleId, string residentId, DateOnly date, CallShiftType shiftType, IEnumerable<DateOnly>? excludedDatesForOverworkConstraints = null)
    {
        excludedDatesForOverworkConstraints ??= [];

        //validate schedule
        Schedule? schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            throw new ArgumentException($"Schedule {scheduleId} not found");
        }

        ResidentDto? residentInfo = await _schedulerService.LoadResidentData(date.Year, date.Semester, residentId, scheduleId);

        if (residentInfo == null)
        {
            throw new ArgumentException($"Resident {residentId} not found");
        }

        foreach (DateOnly excludedDate in excludedDatesForOverworkConstraints)
        {
            residentInfo.AddPendingRemovalWorkDay(excludedDate);
        }

        List<ConstraintResult> violations = [];
        // check through all constrains for any rule violations
        foreach (ICallShiftConstraint constraint in _constraints)
        {
            ConstraintResult result = constraint.Evaluate(residentInfo, date, shiftType);
            if (result.IsViolated)
            {
                violations.Add(result);
            }
        }

        return new ViolationResult(violations);
    }
}