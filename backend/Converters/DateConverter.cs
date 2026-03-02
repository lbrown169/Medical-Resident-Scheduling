using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class DateConverter
{
    public Date CreateDateFromDateCreateRequest(DateCreateRequest request)
    {
        return new Date
        {
            DateId = Guid.NewGuid(),
            ScheduleId = request.ScheduleId,
            ResidentId = request.ResidentId,
            ShiftDate = request.ShiftDate,
            CallType = request.CallType,
        };
    }

    /// <summary>
    ///     This method requires the Resident field to be included
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public DateResponse CreateDateResponseFromDate(Date date)
    {
        return new DateResponse
        {
            DateId = date.DateId,
            ScheduleId = date.ScheduleId,
            ResidentId = date.ResidentId,
            FirstName = date.Resident.FirstName,
            LastName = date.Resident.LastName,
            ShiftDate = date.ShiftDate,
            CallType = new DateCallTypeShiftResponse(date.CallType),
            Hours = date.Hours,
        };
    }

    public void UpdateDateFromDateUpdateRequest(Date date,
        DateUpdateRequest updatedDate)
    {
        date.ScheduleId = updatedDate.ScheduleId ?? date.ScheduleId;
        date.ResidentId = updatedDate.ResidentId ?? date.ResidentId;
        date.ShiftDate = updatedDate.ShiftDate ?? date.ShiftDate;
        date.CallType = updatedDate.CallType ?? date.CallType;
    }
}