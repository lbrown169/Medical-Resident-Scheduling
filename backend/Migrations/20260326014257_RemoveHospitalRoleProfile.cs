using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RemoveHospitalRoleProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hospital_role_profile",
                table: "residents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "hospital_role_profile",
                table: "residents",
                type: "int",
                nullable: true);
        }
    }
}