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

        if (!success && message != null)
        {
            return BadRequest(new AlgorithmResponse
            {
                Success = false,
            });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = message
        });
    }


    [HttpPost("{year}/fall")]
    public async Task<IActionResult> GenerateFallSchedule(int year)
    {
        // validate schedule generation requirements
        (bool requirementsMet, string? requirementsMessage) = await _schedulerService.CheckScheduleRequirements(year, Semester.Fall);

        if (!requirementsMet)
        {
            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = requirementsMessage
            });
        }

        // Generate the fall schedule
        (bool success, string? error, ScheduleResponse? schedule)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Fall);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse
            {
                Success = false,
                Message = error ?? "Failed to generate fall schedule"
            });
        }

        return Ok(new
        {
            Success = true,
            Message = "Fall Schedule generated and saved successfully.",
            Data = schedule
        });
    }

    [HttpPost("{year}/spring")]
    public async Task<IActionResult> GenerateSpringSchedule(int year)
    {
        // validate resident hospital role profile assigned
        (bool requirementsMet, string? requirementsMessage) = await _schedulerService.CheckScheduleRequirements(year, Semester.Fall);

        if (!requirementsMet)
        {
            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = requirementsMessage
            });
        }

        // Generate the spring schedule
        (bool success, string? error, ScheduleResponse? schedule)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Spring);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse
            {
                Success = false,
                Message = error ?? "Failed to generate spring schedule"
            });
        }

        return Ok(new
        {
            Success = true,
            Message = "Spring Schedule generated and saved successfully.",
            Data = schedule
        });
    }
}