using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class VacationConverter
{
    public Vacation CreateVacationFromVacationCreateRequest(VacationCreateRequest request)
    {
        return new Vacation
        {
            VacationId = Guid.NewGuid(),
            ResidentId = request.ResidentId,
            Date = request.Date,
            Reason = request.Reason,
            Status = nameof(RequestStatus.Pending),
            Details = request.Details,
            GroupId = request.GroupId,
            HalfDay = request.HalfDay,
        };
    }

    public VacationResponse CreateVacationResponseFromVacation(Vacation vacation)
    {
        return new VacationResponse
        {
            VacationId = vacation.VacationId,
            ResidentId = vacation.ResidentId,
            Date = vacation.Date,
            Reason = vacation.Reason,
            Status = vacation.Status,
            Details = vacation.Details,
            GroupId = vacation.GroupId,
            HalfDay = vacation.HalfDay,
        };
    }

    public void UpdateVacationFromVacationUpdateRequest(Vacation vacation,
        VacationUpdateRequest request)
    {
        vacation.Date = request.Date ?? vacation.Date;
        vacation.Reason = request.Reason ?? vacation.Reason;
        vacation.Details = request.Details ?? vacation.Details;
        vacation.HalfDay = request.HalfDay ?? vacation.HalfDay;
    }
}