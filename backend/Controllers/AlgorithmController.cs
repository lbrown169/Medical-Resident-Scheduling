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

    [HttpPost("training/{year}")]
    public async Task<IActionResult> GenerateFullSchedule(int year)
    {
        if (year < (DateTime.Now.Year - 1))
        {
            return BadRequest(new
            {
                success = false,
                error = "Year must be the current year or later."
            });
        }

        // validate resident hospital role profile assigned
        bool hasMissingProfiles = await _context.Residents
            .AnyAsync(r => !r.HospitalRoleProfile.HasValue);

        if (hasMissingProfiles)
        {
            List<string> missingList = await _context.Residents
                .Where(r => !r.HospitalRoleProfile.HasValue)
                .Select(r => $"{r.FirstName} {r.LastName} (PGY{r.GraduateYr})")
                .ToListAsync();

            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = $"Invalid Input - Cannot generate schedules: {missingList.Count} resident(s) missing hospital role profiles"
            });
        }

        // Generate the new schedule
        (bool success, string? error)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Fall);
        await _schedulerService.GenerateScheduleForSemester(year, Semester.Spring);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse { Success = false, Message = error ?? $"Failed to generate {year} schedule"});
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = "Schedule generated and saved successfully."
        });
    }

    [HttpPost("{year}/fall")]
    public async Task<IActionResult> GenerateFallSchedule(int year)
    {
        if (year < (DateTime.Now.Year - 1))
        {
            return BadRequest(new
            {
                success = false,
                error = "Year must be the current year or later."
            });
        }

        // validate resident hospital role profile assigned
        bool hasMissingProfiles = await _context.Residents
            .AnyAsync(r => !r.HospitalRoleProfile.HasValue);

        if (hasMissingProfiles)
        {
            List<string> missingList = await _context.Residents
                .Where(r => !r.HospitalRoleProfile.HasValue)
                .Select(r => $"{r.FirstName} {r.LastName} (PGY{r.GraduateYr})")
                .ToListAsync();

            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = $"Invalid Input - Cannot generate schedules: {missingList.Count} resident(s) missing hospital role profiles"
            });
        }

        // Generate the fall schedule
        (bool success, string? error)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Fall);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse
            {
                Success = false,
                Message = error ?? "Failed to generate fall schedule"
            });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = "Fall Schedule generated and saved successfully."
        });
    }

    [HttpPost("{year}/spring")]
    public async Task<IActionResult> GenerateSpringSchedule(int year)
    {
        if (year < (DateTime.Now.Year - 1))
        {
            return BadRequest(new
            {
                success = false,
                error = "Year must be the current year or later."
            });
        }

        // validate resident hospital role profile assigned
        bool hasMissingProfiles = await _context.Residents
            .AnyAsync(r => !r.HospitalRoleProfile.HasValue);

        if (hasMissingProfiles)
        {
            List<string> missingList = await _context.Residents
                .Where(r => !r.HospitalRoleProfile.HasValue)
                .Select(r => $"{r.FirstName} {r.LastName} (PGY{r.GraduateYr})")
                .ToListAsync();

            return BadRequest(new AlgorithmResponse
            {
                Success = false,
                Message = $"Invalid Input - Cannot generate schedules: {missingList.Count} resident(s) missing hospital role profiles"
            });
        }

        // Generate the spring schedule
        (bool success, string? error)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Spring);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse
            {
                Success = false,
                Message = error ?? "Failed to generate spring schedule"
            });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = "Spring Schedule generated and saved successfully."
        });
    }
}