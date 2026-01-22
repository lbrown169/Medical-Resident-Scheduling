using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class DbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "rotation_types",
              columns: table => new
              {
                  rotation_type_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                  rotation_name = table.Column<string>(type: "longtext", nullable: false)
                      .Annotation("MySql:CharSet", "utf8mb4"),
                  does_long_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                  does_short_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                  does_training_long_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                  does_training_short_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                  is_chief_rotation = table.Column<bool>(type: "tinyint(1)", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_rotation_types", x => x.rotation_type_id);
              })
              .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
               table: "rotation_types",
               columns: new[] { "rotation_type_id", "does_long_call", "does_short_call", "does_training_long_call", "does_training_short_call", "is_chief_rotation", "rotation_name" },
               values: new object[] { new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 }, false, false, false, false, false, "Inpatient Psy" });

            migrationBuilder.InsertData(
                table: "rotation_types",
                columns: new[] { "rotation_type_id", "does_long_call", "does_short_call", "does_training_long_call", "does_training_short_call", "is_chief_rotation", "rotation_name" },
                values: new object[] { new byte[] { 215, 87, 101, 160, 153, 16, 236, 66, 156, 238, 237, 62, 236, 98, 19, 171 }, false, false, false, false, false, "Consult" });

            migrationBuilder.DropColumn(
                name: "rotation",
                table: "rotations");

            migrationBuilder.AddColumn<int>(
                name: "month_index",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pgy_year",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "rotation_type_id",
                table: "rotations",
                type: "binary(16)",
                nullable: false,
                defaultValue: new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 });

            migrationBuilder.AddColumn<int>(
                name: "year",
                table: "rotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rotations_rotation_type_id",
                table: "rotations",
                column: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotations_rotation_types_rotation_type_id",
                table: "rotations",
                column: "rotation_type_id",
                principalTable: "rotation_types",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotations_rotation_types_rotation_type_id",
                table: "rotations");

            migrationBuilder.DropTable(
                name: "rotation_types");

            migrationBuilder.DropIndex(
                name: "IX_rotations_rotation_type_id",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "month_index",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "pgy_year",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "rotation_type_id",
                table: "rotations");

            migrationBuilder.DropColumn(
                name: "year",
                table: "rotations");

            migrationBuilder.AddColumn<string>(
                name: "rotation",
                table: "rotations",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}