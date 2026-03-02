using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class UpdatePgy4RotationScheduleTableColumnKeyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.RenameColumn(
                name: "PGY4RotationScheduleId",
                table: "rotations",
                newName: "Pgy4RotationScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_rotations_PGY4RotationScheduleId",
                table: "rotations",
                newName: "IX_rotations_Pgy4RotationScheduleId");

            migrationBuilder.RenameColumn(
                name: "PGY4RotationScheduleId",
                table: "pgy4_rotation_schedule",
                newName: "Pgy4RotationScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_Pgy4RotationScheduleId",
                table: "rotations",
                column: "Pgy4RotationScheduleId",
                principalTable: "pgy4_rotation_schedule",
                principalColumn: "Pgy4RotationScheduleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_Pgy4RotationScheduleId",
                table: "rotations");

            migrationBuilder.RenameColumn(
                name: "Pgy4RotationScheduleId",
                table: "rotations",
                newName: "PGY4RotationScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_rotations_Pgy4RotationScheduleId",
                table: "rotations",
                newName: "IX_rotations_PGY4RotationScheduleId");

            migrationBuilder.RenameColumn(
                name: "Pgy4RotationScheduleId",
                table: "pgy4_rotation_schedule",
                newName: "PGY4RotationScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations",
                column: "PGY4RotationScheduleId",
                principalTable: "pgy4_rotation_schedule",
                principalColumn: "PGY4RotationScheduleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}