using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationCompositeKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rotations",
                table: "rotations");

            migrationBuilder.DropIndex(
                name: "rotation_id_UNIQUE",
                table: "rotations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rotations",
                table: "rotations",
                columns: new[] { "rotation_id", "AcademicMonthIndex" });

            migrationBuilder.CreateIndex(
                name: "rotation_id_month_Unique",
                table: "rotations",
                columns: new[] { "rotation_id", "AcademicMonthIndex" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rotations",
                table: "rotations");

            migrationBuilder.DropIndex(
                name: "rotation_id_month_Unique",
                table: "rotations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rotations",
                table: "rotations",
                column: "rotation_id");

            migrationBuilder.CreateIndex(
                name: "rotation_id_UNIQUE",
                table: "rotations",
                column: "rotation_id",
                unique: true);
        }
    }
}