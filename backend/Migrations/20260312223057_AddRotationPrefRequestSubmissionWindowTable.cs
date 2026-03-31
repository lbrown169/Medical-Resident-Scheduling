using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class AddRotationPrefRequestSubmissionWindowTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rotation_pref_request_submission_window",
                columns: table => new
                {
                    SubmissionWindowId = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    AvailableDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rotation_pref_request_submission_window", x => x.SubmissionWindowId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rotation_pref_request_submission_window");
        }
    }
}