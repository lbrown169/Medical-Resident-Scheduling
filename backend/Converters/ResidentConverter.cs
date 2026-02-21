using MedicalDemo.Enums;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class ResidentConverter
{
    public ResidentResponse CreateResidentResponseFromResident(
        Resident resident)
    {
        string? chiefType = resident.ChiefType switch
        {
            ChiefType.None => null,
            ChiefType.Admin => "Admin",
            ChiefType.Clinic => "Clinic",
            ChiefType.Education => "Education",
            _ => null
        };

        return new ResidentResponse
        {
            resident_id = resident.ResidentId,
            first_name = resident.FirstName,
            last_name = resident.LastName,
            graduate_yr = resident.GraduateYr,
            email = resident.Email,
            phone_num = resident.PhoneNum,
            weekly_hours = resident.WeeklyHours,
            total_hours = resident.TotalHours,
            bi_yearly_hours = resident.BiYearlyHours,
            hospital_role_profile = resident.HospitalRoleProfile,
            chief_type = chiefType
        };
    }

    public void UpdateResidentWithResidentUpdateRequest(Resident resident,
        ResidentUpdateRequest request)
    {
        resident.FirstName = request.first_name ?? resident.FirstName;
        resident.LastName = request.last_name ?? resident.LastName;
        resident.GraduateYr = request.graduate_yr ?? resident.GraduateYr;
        resident.Email = request.email ?? resident.Email;
        resident.PhoneNum = request.phone_num ?? resident.PhoneNum;
        resident.HospitalRoleProfile = request.hospital_role_profile ?? resident.HospitalRoleProfile; ;
    }
}