using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/algorithm")]
public class ScheduleController : ControllerBase
{
    private readonly SchedulerService _schedulerService;

    public ScheduleController(SchedulerService schedulerService)
    {
        _schedulerService = schedulerService;
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

        // Generate the new schedule
        (bool success, string? error)
            = await _schedulerService.GenerateScheduleForSemester(year, Semester.Fall);
        if (!success)
        {
            return StatusCode(500, new AlgorithmResponse { Success = false, Message = error });
        }

        return Ok(new AlgorithmResponse
        {
            Success = true,
            Message = "Schedule generated and saved successfully."
        });
    }
}