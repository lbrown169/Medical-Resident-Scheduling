using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class Pgy4RotationOverrideMonthOfYearStandardization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcademicMonthIndexOverride",
                table: "pgy4_rotation_schedule_override",
                newName: "RotationMonthOfYearOverride");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RotationMonthOfYearOverride",
                table: "pgy4_rotation_schedule_override",
                newName: "AcademicMonthIndexOverride");
        }
    }
}