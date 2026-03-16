using MedicalDemo.Enums;
using MedicalDemo.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
#pragma warning disable CS0618 // Type or member is obsolete

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class SeedRotationsTable : Migration
    {
        private static readonly string[] columns = new[] { "rotation_id", "AcademicYear", "AcademicMonthIndex", "Pgy4RotationScheduleId", "resident_id", "month", "PgyYear", "RotationTypeId" };
        private static readonly string[] keyColumns = new string[] { "rotation_id", "AcademicYear" };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "rotation",
            //     table: "rotations");

            migrationBuilder.AlterColumn<string>(
                name: "resident_id",
                table: "rotations",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: false);

            migrationBuilder.InsertData(
                table: "rotations",
                columns: columns,
                values: SeedingData.Pgy1Rotations);

            migrationBuilder.InsertData(
                table: "rotations",
                columns: columns,
                values: SeedingData.Pgy2Rotations);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM rotations WHERE PgyYear = 1 AND AcademicYear = 2025");
            migrationBuilder.Sql("DELETE FROM rotations WHERE PgyYear = 2 AND AcademicYear = 2025");

            migrationBuilder.AlterColumn<string>(
                name: "resident_id",
                table: "rotations",
                type: "varchar(15)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rotation",
                table: "rotations",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}