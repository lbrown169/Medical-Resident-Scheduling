using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class AddPGY4RotationScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "rotations",
                newName: "Pgy4RotationScheduleId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pgy4_rotation_schedule");

            migrationBuilder.RenameColumn(
                name: "Pgy4RotationScheduleId",
                table: "rotations",
                newName: "ScheduleId");
        }
    }
}
