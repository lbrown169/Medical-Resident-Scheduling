using MedicalDemo.Converters;
using MedicalDemo.Extensions;
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

        int academicYear = pgy4RotationScheduleService.GetAcademicYear();

        DateTime adjustedAvailableDate = new(
            request.AvailableDate.Year,
            request.AvailableDate.Month,
            request.AvailableDate.Day,
            request.AvailableDate.Hour,
            request.AvailableDate.Minute,
            request.AvailableDate.Second
        );
        DateTime adjustedDueDate = new(
            request.DueDate.Year,
            request.DueDate.Month,
            request.DueDate.Day,
            request.DueDate.Hour,
            request.DueDate.Minute,
            request.DueDate.Second
        );

        request.AvailableDate = adjustedAvailableDate;
        request.DueDate = adjustedDueDate;

        bool validAvailableDate = ValidateAvailableDate(adjustedAvailableDate, academicYear);
        bool validDueDate = ValidateDueDate(adjustedAvailableDate, adjustedDueDate, academicYear);
        if (!validAvailableDate || !validDueDate)
        {
            return BadRequest(ModelState);
        }

        RotationPrefSubmissionWindow? submissionWindow =
            await context.RotationPrefRequestSubmissionWindows.FirstOrDefaultAsync(
                (w) => w.AcademicYear == academicYear + 1
            );

        if (submissionWindow == null)
        {
            // Add new submission window to DB
            submissionWindow = submissionWindowConverter.CreateModelFromRequest(
                request,
                academicYear + 1
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
    public async Task<
        ActionResult<RotationPrefSubmissionWindowResponse>
    > GetCurrentRotationPrefSubmissionWindow()
    {
        int academicYear = pgy4RotationScheduleService.GetAcademicYear();

        RotationPrefSubmissionWindow? submissionWindow =
            await context.RotationPrefRequestSubmissionWindows.FirstOrDefaultAsync(
                (w) => w.AcademicYear == academicYear + 1
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
        if (availableDateYear == academicYear && availableDateMonth >= 7)
        {
            return true;
        }
        else if (availableDateYear == academicYear + 1 && availableDateMonth < 7)
        {
            return true;
        }

        ModelState.AddModelError(
            "Invalid dates",
            $"Available date must be between {7}/{academicYear} and {6}/{academicYear + 1}"
        );
        return false;
    }

    private bool ValidateDueDate(DateTime availableDate, DateTime dueDate, int academicYear)
    {
        
        int dueDateYear = dueDate.Year;
        int dueDateMonth = dueDate.Month;
        bool hasError = false;

        if (availableDate >= dueDate)
        {
            ModelState.AddModelError(
                "Invalid Timespan",
                "Available date cannot be the same as or after due date"
            );
            hasError = true;
        }

        if (
            !(
                dueDateYear == academicYear && dueDateMonth >= 7
                || dueDateYear == academicYear + 1 && dueDateMonth < 7
            )
        )
        {
            ModelState.AddModelError(
                "Invalid dates",
                $"Due date must be between {7}/{academicYear} and {6}/{academicYear + 1}"
            );
            hasError = true;
        }

        if (hasError)
        {
            return false;
        }

        return true;
    }
}