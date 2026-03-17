using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/rotation-request-submission-window")]
public class RotationPrefRequestSubmissionWindowController(
    MedicalContext context,
    Pgy4RotationScheduleService pgy4RotationScheduleService,
    RotationPrefSubmissionWindowConverter submissionWindowConverter
) : ControllerBase
{
    private readonly MedicalContext context = context;
    private readonly Pgy4RotationScheduleService pgy4RotationScheduleService =
        pgy4RotationScheduleService;
    private readonly RotationPrefSubmissionWindowConverter submissionWindowConverter =
        submissionWindowConverter;

    [HttpPost]
    public async Task<ActionResult<RotationPrefSubmissionWindowResponse>> SetSubmissionWindow(
            [FromBody] RotationPrefSubmissionWindowRequest request
        )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int academicYear = pgy4RotationScheduleService.GetScheduleYear();

        bool validAvailableDate = ValidateAvailableDate(request.AvailableDate, academicYear);
        bool validDueDate = ValidateDueDate(request.AvailableDate, request.DueDate, academicYear);
        if (!validAvailableDate || !validDueDate)
        {
            return BadRequest(ModelState);
        }

        RotationPrefSubmissionWindow? submissionWindow =
            await context.RotationPrefRequestSubmissionWindows.FirstOrDefaultAsync(
                (w) => w.AcademicYear == academicYear
            );

        if (submissionWindow == null)
        {
            // Add new submission window to DB
            submissionWindow = submissionWindowConverter.CreateModelFromRequest(
                request,
                academicYear
            );

            await context.RotationPrefRequestSubmissionWindows.AddAsync(submissionWindow);
            await context.SaveChangesAsync();
        }
        else
        {
            // Update existing submission window
            submissionWindow.AvailableDate = request.AvailableDate;
            submissionWindow.DueDate = request.DueDate;

            await context.SaveChangesAsync();
        }

        // Parse to response
        RotationPrefSubmissionWindowResponse response =
            submissionWindowConverter.CreateResponseFromModel(submissionWindow);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<RotationPrefSubmissionWindowResponse>> GetCurrentRotationPrefSubmissionWindow()
    {
        int academicYear = pgy4RotationScheduleService.GetScheduleYear();

        RotationPrefSubmissionWindow? submissionWindow =
            await context.RotationPrefRequestSubmissionWindows.FirstOrDefaultAsync(
                (w) => w.AcademicYear == academicYear
            );

        if (submissionWindow == null)
        {
            return NotFound();
        }

        // Parse to response
        RotationPrefSubmissionWindowResponse response =
            submissionWindowConverter.CreateResponseFromModel(submissionWindow);
        return Ok(response);
    }

    private bool ValidateAvailableDate(DateTime availableDate, int academicYear)
    {
        int availableDateYear = availableDate.Year;
        int availableDateMonth = availableDate.Month;
        if (availableDateYear == academicYear && availableDateMonth <= 6)
        {
            return true;
        }
        else if (availableDateYear == academicYear - 1 && availableDateMonth >= 7)
        {
            return true;
        }

        ModelState.AddModelError(
            "Invalid dates",
            $"Available date must be between {7}/{academicYear - 1} and {6}/{academicYear}"
        );
        return false;
    }

    private bool ValidateDueDate(DateTime availableDate, DateTime dueDate, int academicYear)
    {
        availableDate = new DateTime(availableDate.Year, availableDate.Month, availableDate.Day, 0, 0, 0);
        dueDate = new DateTime(dueDate.Year, dueDate.Month, dueDate.Day, 0, 0, 0);

        int dueDateYear = dueDate.Year;
        int dueDateMonth = dueDate.Month;
        bool hasError = false;

        if (availableDate >= dueDate)
        {
            ModelState.AddModelError("Invalid Timespan", "Available date cannot be the same as or after due date");
            hasError = true;
        }

        if (!hasError)
        {
            if (dueDateYear == academicYear && dueDateMonth <= 6)
            {
                return true;
            }
            else if (dueDateYear == academicYear - 1 && dueDateMonth >= 7)
            {
                return true;
            }
        }

        ModelState.AddModelError(
            "Invalid dates",
            $"Due date must be between {7}/{academicYear - 1} and {6}/{academicYear}"
        );

        return false;
    }
}