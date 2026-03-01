using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class CreatePgy4RotationScheduleOverrideTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pgy4_rotation_schedule_override",
                columns: table => new
                {
                    Pgy4RotationScheduleOverrideId = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    Pgy4RotationScheduleId = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    AcademicMonthIndexOverride = table.Column<int>(type: "int", nullable: false),
                    ResidentOverrideId = table.Column<string>(type: "varchar(15)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RotationTypeOverrideId = table.Column<byte[]>(type: "binary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pgy4_rotation_schedule_override", x => x.Pgy4RotationScheduleOverrideId);
                    table.ForeignKey(
                        name: "FK_pgy4_rotation_schedule_override_pgy4_rotation_schedule_Pgy4R~",
                        column: x => x.Pgy4RotationScheduleId,
                        principalTable: "pgy4_rotation_schedule",
                        principalColumn: "Pgy4RotationScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pgy4_rotation_schedule_override_residents_ResidentOverrideId",
                        column: x => x.ResidentOverrideId,
                        principalTable: "residents",
                        principalColumn: "resident_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pgy4_rotation_schedule_override_rotation_type_RotationTypeOv~",
                        column: x => x.RotationTypeOverrideId,
                        principalTable: "rotation_type",
                        principalColumn: "RotationTypeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_pgy4_rotation_schedule_override_Pgy4RotationScheduleId",
                table: "pgy4_rotation_schedule_override",
                column: "Pgy4RotationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_pgy4_rotation_schedule_override_ResidentOverrideId",
                table: "pgy4_rotation_schedule_override",
                column: "ResidentOverrideId");

            migrationBuilder.CreateIndex(
                name: "IX_pgy4_rotation_schedule_override_RotationTypeOverrideId",
                table: "pgy4_rotation_schedule_override",
                column: "RotationTypeOverrideId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pgy4_rotation_schedule_override");
        }
    }
}
