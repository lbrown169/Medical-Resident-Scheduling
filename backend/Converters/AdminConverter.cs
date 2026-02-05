using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;

namespace MedicalDemo.Converters;

public class AdminConverter
{
    public void UpdateAdminFromAdminUpdateRequest(Admin admin,
        AdminUpdateRequest updateRequest)
    {
        admin.FirstName = updateRequest.FirstName ?? admin.FirstName;
        admin.LastName = updateRequest.LastName ?? admin.LastName;
        admin.Email = updateRequest.Email ?? admin.Email;
        admin.PhoneNum = updateRequest.PhoneNum ?? admin.PhoneNum; ;
    }

    public AdminResponse CreateAdminResponseFromAdmin(Admin admin)
    {
        AdminResponse response = new()
        {
            admin_id = admin.AdminId,
            first_name = admin.FirstName,
            last_name = admin.LastName,
            email = admin.Email,
            phone_num = admin.PhoneNum
        };

        return response;
    }

    public Admin CreateAdminFromResident(Resident resident)
    {
        Admin newAdmin = new()
        {
            AdminId = resident.ResidentId,
            FirstName = resident.FirstName,
            LastName = resident.LastName,
            Email = resident.Email,
            Password = resident.Password,
            PhoneNum = resident.PhoneNum
        };

        return newAdmin;
    }
}