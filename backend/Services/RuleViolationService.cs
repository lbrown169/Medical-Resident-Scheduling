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
    private readonly IList<ICallShiftConstraint> _constraints;
    private readonly AlgorithmService _algorithmService;
    private readonly MedicalContext _context;
    private readonly SchedulingMapperService _mapper;
    private readonly SchedulerService _schedulerService;

    public RuleViolationService(
        MedicalContext context,
        SchedulingMapperService mapper,
        AlgorithmService algorithmService,
        ILogger<SchedulerService> logger,
        IList<ICallShiftConstraint> constraints,
        SchedulerService schedulerService
    )
    {
        _context = context;
        _mapper = mapper;
        _algorithmService = algorithmService;
        _logger = logger;
        _constraints = constraints.ToList();
        _schedulerService = schedulerService;
    }


    public async Task<(bool Success, string? Error, Resident? resident)> CheckResidentScheduledOnDate(
        Guid schedule_id,
        string resident_id,
        DateOnly date
        )
    {
        //validate schedule and resident
        Schedule? schedule = await _context.Schedules.FindAsync(schedule_id);
        if (schedule == null)
        {
            return (false, "Invalid ScheduleID.", null);
        }

        Resident? resident = await _context.Residents.FindAsync(resident_id);
        if (resident == null)
        {
            return (false, "Invalid ResidentID.", null);
        }

        // check if shift exists for this resident on this day
        bool residentScheduled = await _context.Dates
            .AnyAsync(d =>
                d.ScheduleId == schedule_id &&
                d.ResidentId == resident_id &&
                d.ShiftDate == date);

        if (residentScheduled)
        {
            return (false, $"Resident is already scheduled on this date. Resident: {resident_id}, Schedule:{schedule_id},Date:{date}", resident);
        }

        return (true, null, resident);
    }

    public async Task<ViolationResult> EvaluateConstraints(Guid schedule_id, string resident_id, DateOnly date)
    {
        //validate schedule and resident
        Schedule? schedule = await _context.Schedules.FindAsync(schedule_id);
        if (schedule == null)
        {
            throw new ArgumentException($"Invalid ScheduleID {schedule_id}.");
        }

        Resident resident = await _context.Residents.FindAsync(resident_id);
        if (resident == null)
        {
            throw new ArgumentException($"Invalid ResidentID {resident_id}.");
        }

        ResidentDto residentInfo = await _schedulerService.LoadResidentData(date.AcademicYear, date.Semester, resident_id);
        List<ConstraintResult> violations = new();

        if (residentInfo == null)
        {
            throw new Exception($"Resident Data for {resident_id} not found.");
        }

        // check through all constrains for any rule violations
        foreach (ICallShiftConstraint constraint in _constraints)
        {
            ConstraintResult result = constraint.Evaluate(residentInfo, date);
            if (result.IsViolated)
            {
                violations.Add(result);
            }
        }

        return new ViolationResult(violations);
    }
}