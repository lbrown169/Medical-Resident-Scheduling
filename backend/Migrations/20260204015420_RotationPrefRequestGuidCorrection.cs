using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationPrefRequestGuidCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "third_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "third_avoid_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "third_alternative_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "sixth_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "seventh_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "second_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "second_avoid_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "second_alternative_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "fourth_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "first_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "first_avoid_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "first_alternative_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "fifth_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<byte[]>(
                name: "eighth_priority_id",
                table: "rotation_pref_request",
                type: "binary(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "additional_notes",
                table: "rotation_pref_request",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_eighth_priority_id",
                table: "rotation_pref_request",
                column: "eighth_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_fifth_priority_id",
                table: "rotation_pref_request",
                column: "fifth_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_first_alternative_id",
                table: "rotation_pref_request",
                column: "first_alternative_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_first_avoid_id",
                table: "rotation_pref_request",
                column: "first_avoid_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_first_priority_id",
                table: "rotation_pref_request",
                column: "first_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_fourth_priority_id",
                table: "rotation_pref_request",
                column: "fourth_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_second_alternative_id",
                table: "rotation_pref_request",
                column: "second_alternative_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_second_avoid_id",
                table: "rotation_pref_request",
                column: "second_avoid_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_second_priority_id",
                table: "rotation_pref_request",
                column: "second_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_seventh_priority_id",
                table: "rotation_pref_request",
                column: "seventh_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_sixth_priority_id",
                table: "rotation_pref_request",
                column: "sixth_priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_third_alternative_id",
                table: "rotation_pref_request",
                column: "third_alternative_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_third_avoid_id",
                table: "rotation_pref_request",
                column: "third_avoid_id");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_third_priority_id",
                table: "rotation_pref_request",
                column: "third_priority_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_eighth_priority_id",
                table: "rotation_pref_request",
                column: "eighth_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_fifth_priority_id",
                table: "rotation_pref_request",
                column: "fifth_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_alternative_id",
                table: "rotation_pref_request",
                column: "first_alternative_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_avoid_id",
                table: "rotation_pref_request",
                column: "first_avoid_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_priority_id",
                table: "rotation_pref_request",
                column: "first_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_fourth_priority_id",
                table: "rotation_pref_request",
                column: "fourth_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_alternative_id",
                table: "rotation_pref_request",
                column: "second_alternative_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_avoid_id",
                table: "rotation_pref_request",
                column: "second_avoid_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_priority_id",
                table: "rotation_pref_request",
                column: "second_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_seventh_priority_id",
                table: "rotation_pref_request",
                column: "seventh_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_sixth_priority_id",
                table: "rotation_pref_request",
                column: "sixth_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_alternative_id",
                table: "rotation_pref_request",
                column: "third_alternative_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_avoid_id",
                table: "rotation_pref_request",
                column: "third_avoid_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_priority_id",
                table: "rotation_pref_request",
                column: "third_priority_id",
                principalTable: "rotation_type",
                principalColumn: "rotation_type_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_eighth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_fifth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_first_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_fourth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_second_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_seventh_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_sixth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_third_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_eighth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_fifth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_first_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_first_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_first_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_fourth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_second_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_second_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_second_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_seventh_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_sixth_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_third_alternative_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_third_avoid_id",
                table: "rotation_pref_request");

            migrationBuilder.DropIndex(
                name: "IX_rotation_pref_request_third_priority_id",
                table: "rotation_pref_request");

            migrationBuilder.AlterColumn<Guid>(
                name: "third_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "third_avoid_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "third_alternative_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "sixth_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "seventh_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "second_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "second_avoid_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "second_alternative_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "fourth_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "first_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "first_avoid_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "first_alternative_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "fifth_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "eighth_priority_id",
                table: "rotation_pref_request",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(byte[]),
                oldType: "binary(16)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "rotation_pref_request",
                keyColumn: "additional_notes",
                keyValue: null,
                column: "additional_notes",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "additional_notes",
                table: "rotation_pref_request",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}