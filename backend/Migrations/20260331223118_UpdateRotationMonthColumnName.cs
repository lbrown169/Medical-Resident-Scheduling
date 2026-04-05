using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class UpdateRotationMonthColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AcademicMonthIndex",
                table: "rotations",
                newName: "RotationMonthOfYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RotationMonthOfYear",
                table: "rotations",
                newName: "AcademicMonthIndex");
        }
    }
}