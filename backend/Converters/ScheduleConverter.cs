using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using MedicalDemo.Extensions;

namespace MedicalDemo.Converters;

public class ScheduleConverter
{
    public ScheduleResponse CreateScheduleResponseFromSchedule(
        Schedule schedule)
    {
        ScheduleResponse response = new ScheduleResponse
        {
            ScheduleId = schedule.ScheduleId,
            Status = new ScheduleStatusResponse(schedule.Status),
            GeneratedYear = schedule.GeneratedYear,
            Semester = new SemesterInfoResponse(schedule.Semester)
        };

        if (schedule.Dates.Any())
        {
            response.ResidentHours = schedule.Dates
                .GroupBy(d => d.ResidentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(d => d.Hours)
                );
            response.TotalHours = response.ResidentHours.Values.Sum();
            response.TotalResidents = response.ResidentHours.Count();
        }
        return response;
    }
}