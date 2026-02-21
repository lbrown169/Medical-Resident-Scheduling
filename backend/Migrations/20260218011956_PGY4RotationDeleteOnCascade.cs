using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class PGY4RotationDeleteOnCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations",
                column: "PGY4RotationScheduleId",
                principalTable: "pgy4_rotation_schedule",
                principalColumn: "PGY4RotationScheduleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations",
                column: "PGY4RotationScheduleId",
                principalTable: "pgy4_rotation_schedule",
                principalColumn: "PGY4RotationScheduleId");
        }
    }
}