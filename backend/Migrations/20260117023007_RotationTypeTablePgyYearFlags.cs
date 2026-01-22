using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationTypeTablePgyYearFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "rotation_type_id",
                table: "rotations",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 },
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldDefaultValue: new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 });

            migrationBuilder.AddColumn<int>(
                name: "pgy_year_flags",
                table: "rotation_types",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pgy_year_flags",
                table: "rotation_types");

            migrationBuilder.AlterColumn<byte[]>(
                name: "rotation_type_id",
                table: "rotations",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 },
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldDefaultValue: new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 });
        }
    }
}