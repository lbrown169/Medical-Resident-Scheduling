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

    public async Task<ViolationResult> EvaluateConstraints(Guid scheduleId, string residentId, DateOnly date, CallShiftType shiftType, bool isDateUpdate = true, bool isResidentUpdate = true)
    {
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

        List<ConstraintResult> violations = [];
        // check through all constrains for any rule violations
        foreach (ICallShiftConstraint constraint in _constraints)
        {
            // if updating date for same resident, bypass OneShiftADayConstraint
            if (!constraint.IsApplicable(isDateUpdate, isResidentUpdate))
            {
                continue;
            }
            ConstraintResult result = constraint.Evaluate(residentInfo, date, shiftType);
            if (result.IsViolated)
            {
                violations.Add(result);
            }
        }

        return new ViolationResult(violations);
    }
}