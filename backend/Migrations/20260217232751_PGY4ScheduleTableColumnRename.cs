using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class PGY4ScheduleTableColumnRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "seed",
                table: "pgy4_rotation_schedule",
                newName: "Seed");

            migrationBuilder.RenameColumn(
                name: "pgy4_rotation_schedule_id",
                table: "pgy4_rotation_schedule",
                newName: "PGY4RotationScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Seed",
                table: "pgy4_rotation_schedule",
                newName: "seed");

            migrationBuilder.RenameColumn(
                name: "PGY4RotationScheduleId",
                table: "pgy4_rotation_schedule",
                newName: "pgy4_rotation_schedule_id");
        }
    }
}