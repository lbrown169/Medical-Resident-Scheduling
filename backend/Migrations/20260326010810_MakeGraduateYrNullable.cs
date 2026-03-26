using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class MakeGraduateYrNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "resident_id_rotation",
                table: "rotations");

            migrationBuilder.AlterColumn<string>(
                name: "resident_id",
                table: "rotations",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "graduate_yr",
                table: "residents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "'1'");

            migrationBuilder.AddForeignKey(
                name: "resident_id_rotation",
                table: "rotations",
                column: "resident_id",
                principalTable: "residents",
                principalColumn: "resident_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "resident_id_rotation",
                table: "rotations");

            migrationBuilder.UpdateData(
                table: "rotations",
                keyColumn: "resident_id",
                keyValue: null,
                column: "resident_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "resident_id",
                table: "rotations",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "graduate_yr",
                table: "residents",
                type: "int",
                nullable: false,
                defaultValueSql: "'1'",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "resident_id_rotation",
                table: "rotations",
                column: "resident_id",
                principalTable: "residents",
                principalColumn: "resident_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}