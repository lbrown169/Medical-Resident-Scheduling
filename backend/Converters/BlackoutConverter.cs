using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class BlackoutConverter
{
    public Blackout CreateBlackoutFromBlackoutCreateRequest(BlackoutCreateRequest request)
    {
        return new Blackout
        {
            BlackoutId = Guid.NewGuid(),
            Date = request.Date,
            ResidentId = request.ResidentId,
        };
    }

    public BlackoutResponse CreateBlackoutResponseFromBlackout(
        Blackout blackout)
    {
        return new BlackoutResponse
        {
            BlackoutId = blackout.BlackoutId,
            Date = blackout.Date,
            ResidentId = blackout.ResidentId,
        };
    }

    public void UpdateBlackoutFromBlackoutUpdateRequest(Blackout blackout, BlackoutUpdateRequest request)
    {
        blackout.Date = request.Date ?? blackout.Date; ;
        blackout.ResidentId = request.ResidentId ?? blackout.ResidentId;
    }
}