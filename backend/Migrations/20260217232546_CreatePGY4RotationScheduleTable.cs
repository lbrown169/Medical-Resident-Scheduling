using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class CreatePGY4RotationScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PGY4RotationScheduleId",
                table: "rotations",
                type: "binary(16)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChiefType",
                table: "residents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "pgy4_rotation_schedule",
                columns: table => new
                {
                    pgy4_rotation_schedule_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    seed = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pgy4_rotation_schedule", x => x.pgy4_rotation_schedule_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_rotations_PGY4RotationScheduleId",
                table: "rotations",
                column: "PGY4RotationScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations",
                column: "PGY4RotationScheduleId",
                principalTable: "pgy4_rotation_schedule",
                principalColumn: "pgy4_rotation_schedule_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_pgy4_rotation_schedule_PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.DropTable(
                name: "pgy4_rotation_schedule");

            migrationBuilder.DropIndex(
                name: "IX_rotations_PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "PGY4RotationScheduleId",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "ChiefType",
                table: "residents");
        }
    }
}
