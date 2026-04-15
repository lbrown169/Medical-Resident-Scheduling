using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class InvitationConverter
{
    public InviteResponse CreateInvitationResponseFromModel(Invitation invitation, Resident? resident)
    {
        return new InviteResponse()
        {
            ResidentId = invitation.ResidentId,
            FirstName = resident?.FirstName,
            LastName = resident?.LastName,
            Email = invitation.Email,
            Expires = new DateTimeOffset(invitation.Expires, TimeSpan.Zero),
        };
    }
}