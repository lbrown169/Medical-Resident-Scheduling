using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationPrefRequestSubmissionWindowPKChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rotation_pref_request_submission_window",
                table: "rotation_pref_request_submission_window");

            migrationBuilder.DropColumn(
                name: "SubmissionWindowId",
                table: "rotation_pref_request_submission_window");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rotation_pref_request_submission_window",
                table: "rotation_pref_request_submission_window",
                column: "AcademicYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rotation_pref_request_submission_window",
                table: "rotation_pref_request_submission_window");

            migrationBuilder.AddColumn<byte[]>(
                name: "SubmissionWindowId",
                table: "rotation_pref_request_submission_window",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_rotation_pref_request_submission_window",
                table: "rotation_pref_request_submission_window",
                column: "SubmissionWindowId");
        }
    }
}