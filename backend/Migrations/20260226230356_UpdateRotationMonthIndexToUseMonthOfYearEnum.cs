using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class UpdateRotationMonthIndexToUseMonthOfYearEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthIndex",
                table: "rotations",
                newName: "AcademicMonthIndex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcademicMonthIndex",
                table: "rotations",
                newName: "MonthIndex");
        }
    }
}