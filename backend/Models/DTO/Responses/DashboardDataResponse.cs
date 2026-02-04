namespace MedicalDemo.Models.DTO.Responses;

public class DashboardDataResponse
{
    public required string ResidentId { get; set; }
    public required string CurrentRotation { get; set; }
    public required string RotationEndDate { get; set; }
    public required int MonthlyHours { get; set; }
    public List<UpcomingShift> UpcomingShifts { get; set; } = [];
    public List<Activity> RecentActivity { get; set; } = [];
    public List<TeamUpdate> TeamUpdates { get; set; } = [];

    public class UpcomingShift
    {
        public string Date { get; set; } = "";
        public string Type { get; set; } = "";
    }

    public class Activity
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Message { get; set; } = "";
        public string Date { get; set; } = "";
    }

    public class TeamUpdate
    {
        public string Id { get; set; } = "";
        public string Message { get; set; } = "";
        public string Date { get; set; } = "";
    }
}