using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class NullableScheduleIdForRotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ScheduleId",
                table: "rotations",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ScheduleId",
                table: "rotations",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);
        }
    }
}
