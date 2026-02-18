using MedicalDemo.Algorithm;
using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Added for List

// Added for Where, ToList, Any, Select

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ILogger<DashboardController> _logger;
    private readonly MedicalContext _context;

    public DashboardController(MedicalContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/dashboard/resident/{residentId}
    [HttpGet("resident/{residentId}")]
    public async Task<ActionResult<DashboardDataResponse>> GetDashboardData(
        string residentId)
    {
        try
        {
            DashboardDataResponse dashboardData = new()
            {
                ResidentId = residentId,
                CurrentRotation = "No rotation assigned",
                RotationEndDate = "",
                MonthlyHours = 0
            };

            Resident? resident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentId == residentId);

            if (resident == null)
            {
                return NotFound();
            }

            HospitalRole role = HospitalRole.Unassigned;
            int monthIndex = (DateTime.Now.Month + 5) % 12;

            if (resident.HospitalRoleProfile is { } profile)
            {
                int pgyLevel = resident.GraduateYr;

                if (pgyLevel == 1 && profile >= 0 && profile < HospitalRole.Pgy1Profiles.Length)
                {
                    role = HospitalRole.Pgy1Profiles[profile][monthIndex];
                }
                else if (pgyLevel == 2)
                {
                    int pgy2Index = profile - 8;
                    if (pgy2Index >= 0 && pgy2Index < HospitalRole.Pgy2Profiles.Length)
                    {
                        role = HospitalRole.Pgy2Profiles[pgy2Index][monthIndex];
                    }
                }
            }

            dashboardData.CurrentRotation = role.name;

            if (role != HospitalRole.Unassigned)
            {
                DateTime endOfMonth
                    = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                        .AddMonths(1).AddDays(-1);
                dashboardData.RotationEndDate = $"Ends {endOfMonth:MM/dd/yyyy}";
            }

            // Get dates for this resident
            List<Date> userDates = await _context.Dates
                .Where(d =>
                    d.ResidentId == residentId
                    && d.Schedule.Status == ScheduleStatus.Published)
                .ToListAsync();

            // Calculate this month's hours
            List<Date> thisMonthDates = userDates.Where(d =>
                d.ShiftDate.Month == DateTime.Now.Month &&
                d.ShiftDate.Year == DateTime.Now.Year).ToList();
            dashboardData.MonthlyHours
                = thisMonthDates.Sum(d => d.Hours);

            // Get upcoming shifts (next 3)
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            List<Date> futureDates = userDates
                .Where(d => d.ShiftDate >= today)
                .OrderBy(d => d.ShiftDate)
                .Take(3)
                .ToList();

            dashboardData.UpcomingShifts = futureDates.Select(d =>
                new DashboardDataResponse.UpcomingShift
                {
                    Date = d.ShiftDate.ToString("MM/dd/yyyy"),
                    Type = d.CallType.GetDisplayName()
                }).ToList();

            // Generate recent activity
            if (thisMonthDates.Any())
            {
                dashboardData.RecentActivity.Add(new DashboardDataResponse.Activity
                {
                    Id = "1",
                    Type = "schedule",
                    Message
                        = $"You have {thisMonthDates.Count} shifts scheduled this month",
                    Date = "Current month"
                });
            }

            if (futureDates.Any())
            {
                Date nextShift = futureDates.First();
                dashboardData.RecentActivity.Add(new DashboardDataResponse.Activity
                {
                    Id = "2",
                    Type = "upcoming",
                    Message
                        = $"Next shift: {nextShift.CallType.GetDisplayName()} on {nextShift.ShiftDate:MM/dd/yyyy}",
                    Date = "Upcoming"
                });
            }

            // Add pending swap requests for this resident (as requestee)
            List<SwapRequest> pendingSwaps = await _context.SwapRequests
                .Where(s =>
                    s.RequesteeId == residentId && s.Status == RequestStatus.Pending)
                .ToListAsync();
            List<string> requesterIds = pendingSwaps.Select(s => s.RequesterId)
                .Distinct().ToList();
            Dictionary<string, string> requesterMap = await _context
                .Residents
                .Where(r => requesterIds.Contains(r.ResidentId))
                .ToDictionaryAsync(r => r.ResidentId,
                    r => r.FirstName + " " + r.LastName);
            foreach (SwapRequest swap in pendingSwaps)
            {
                string requesterName
                    = requesterMap.ContainsKey(swap.RequesterId)
                        ? requesterMap[swap.RequesterId]
                        : swap.RequesterId;
                dashboardData.RecentActivity.Add(new DashboardDataResponse.Activity
                {
                    Id = swap.SwapRequestId.ToString(),
                    Type = "swap_pending",
                    Message =
                        $"Swap request from {requesterName} for {swap.RequesterDate:MM/dd/yyyy} (your shift: {swap.RequesteeDate:MM/dd/yyyy})",
                    Date = swap.CreatedAt.ToString("MM/dd/yyyy")
                });
            }

            // Add swap requests where this resident is the requester and status is Approved or Denied
            List<SwapRequest> respondedSwaps = await _context.SwapRequests
                .Where(s =>
                    s.RequesterId == residentId && (s.Status == RequestStatus.Approved || s.Status == RequestStatus.Denied))
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();
            // Fetch all requestee IDs for these swaps
            List<string> requesteeIds = respondedSwaps
                .Select(s => s.RequesteeId).Distinct().ToList();
            Dictionary<string, string> requesteeMap = await _context
                .Residents
                .Where(r => requesteeIds.Contains(r.ResidentId))
                .ToDictionaryAsync(r => r.ResidentId,
                    r => r.FirstName + " " + r.LastName);
            foreach (SwapRequest swap in respondedSwaps)
            {
                string requesteeName
                    = requesteeMap.ContainsKey(swap.RequesteeId)
                        ? requesteeMap[swap.RequesteeId]
                        : swap.RequesteeId;
                string message = swap.Status == RequestStatus.Approved
                    ? $"Your swap request for {swap.RequesterDate:MM/dd/yyyy} (with {requesteeName}) was approved."
                    : $"Your swap request for {swap.RequesterDate:MM/dd/yyyy} (with {requesteeName}) was denied. Reason: {swap.Details}";
                dashboardData.RecentActivity.Add(new DashboardDataResponse.Activity
                {
                    Id = swap.SwapRequestId.ToString(),
                    Type = swap.Status == RequestStatus.Approved
                        ? "swap_approved"
                        : "swap_denied",
                    Message = message,
                    Date = swap.UpdatedAt.ToString("MM/dd/yyyy")
                });
            }

            // Add swap activity for the requestee (approver) when a swap is approved
            List<SwapRequest> approvedAsRequestee = await _context
                .SwapRequests
                .Where(s =>
                    s.RequesteeId == residentId && s.Status == RequestStatus.Approved)
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();
            List<string> approvedRequesterIds = approvedAsRequestee
                .Select(s => s.RequesterId).Distinct().ToList();
            Dictionary<string, string> approvedRequesterMap = await _context
                .Residents
                .Where(r => approvedRequesterIds.Contains(r.ResidentId))
                .ToDictionaryAsync(r => r.ResidentId,
                    r => r.FirstName + " " + r.LastName);
            foreach (SwapRequest swap in approvedAsRequestee)
            {
                string requesterName
                    = approvedRequesterMap.ContainsKey(swap.RequesterId)
                        ? approvedRequesterMap[swap.RequesterId]
                        : swap.RequesterId;
                string message
                    = $"You swapped your shift on {swap.RequesteeDate:MM/dd/yyyy} with {requesterName}.";
                dashboardData.RecentActivity.Add(new DashboardDataResponse.Activity
                {
                    Id = swap.SwapRequestId + "-as-approver",
                    Type = "swap_approved",
                    Message = message,
                    Date = swap.UpdatedAt.ToString("MM/dd/yyyy")
                });
            }

            // Add team updates from announcements
            List<Announcement> announcements = await _context
                .Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(5) // Show the 5 most recent announcements
                .ToListAsync();

            foreach (Announcement announcement in announcements)
            {
                dashboardData.TeamUpdates.Add(new DashboardDataResponse.TeamUpdate
                {
                    Id = announcement.AnnouncementId.ToString(),
                    Message = announcement.Message ?? "",
                    Date = announcement.CreatedAt.ToString("MM/dd/yyyy")
                });
            }

            return Ok(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get dashboard info");
            return StatusCode(500,
                $"An error occurred while fetching dashboard data: {ex.Message}");
        }
    }
}