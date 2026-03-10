using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/algorithm")]
public class ScheduleController : ControllerBase
{
    private readonly SchedulerService _schedulerService;
    private readonly MedicalContext _context;

    public ScheduleController(SchedulerService schedulerService, MedicalContext context)
    {
        _schedulerService = schedulerService;
        _context = context;
    }

    // for testing purposes -> logic check is in SchedulerService, implemented in Generate endpoint
    [HttpGet("{year}/{semester}/check")]
    public async Task<IActionResult> CheckScheduleRequirements(int year, Semester semester)
    {
        // requirements logic method
        (bool success, string? message)
            = await _schedulerService.CheckScheduleRequirements(year, semester);

        if (!success)
        {
            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = message
            });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = message
        });
    }

    [HttpPost("{year}/{semester}/generate")]
    public async Task<IActionResult> GenerateSemesterSchedule(int year, Semester semester)
    {
        // validate resident hospital role profile assigned
        (bool requirementsMet, string? requirementsMessage) = await _schedulerService.CheckScheduleRequirements(year, semester);

        if (!requirementsMet)
        {
            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = requirementsMessage
            });
        }

        // Generate the schedule
        (bool success, string? error, ScheduleResponse? schedule)
            = await _schedulerService.GenerateScheduleForSemester(year, semester);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse
            {
                Success = false,
                Message = error ?? $"Failed to generate {semester} schedule",
                Schedule = schedule
            });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = $"{semester} Schedule generated and saved successfully.",
            Schedule = schedule
        });
    }
}