using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class AddRotationTypeFKToRotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthIndex",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PgyYear",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "RotationTypeId",
                table: "rotations",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rotations_RotationTypeId",
                table: "rotations",
                column: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_rotation_type_RotationTypeId",
                table: "rotations",
                column: "RotationTypeId",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_rotation_type_RotationTypeId",
                table: "rotations");

            migrationBuilder.DropIndex(
                name: "IX_rotations_RotationTypeId",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "MonthIndex",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "PgyYear",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "RotationTypeId",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "rotations");
        }
    }
}