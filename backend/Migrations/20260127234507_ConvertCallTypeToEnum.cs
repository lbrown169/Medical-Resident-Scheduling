using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class ConvertCallTypeToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "used",
                table: "invitations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValueSql: "'0'");

            migrationBuilder.AlterColumn<int>(
                name: "call_type",
                table: "dates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "used",
                table: "invitations",
                type: "tinyint(1)",
                nullable: false,
                defaultValueSql: "'0'",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "call_type",
                table: "dates",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
