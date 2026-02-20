using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class ScheduleConverter
{
    public ScheduleResponse CreateScheduleResponseFromSchedule(
        Schedule schedule)
    {
        return new ScheduleResponse
        {
            ScheduleId = schedule.ScheduleId,
            Status = new ScheduleStatusResponse(schedule.Status),
            GeneratedYear = schedule.GeneratedYear,
            Semester = new SemesterInfo
            {
                Id = (int)schedule.Semester,
                Name = schedule.Semester.ToString()
            }
        };
    }
}