using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class CreateRotationPrefRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rotation_pref_request",
                columns: table => new
                {
                    RotationPrefRequestId = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    resident_id = table.Column<string>(type: "varchar(15)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    first_priority_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    second_priority_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    third_priority_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    fourth_priority_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    fifth_priority_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    sixth_priority_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    seventh_priority_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    eighth_priority_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    first_alternative_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    second_alternative_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    third_alternative_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    first_avoid_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    second_avoid_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    third_avoid_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    additional_notes = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rotation_pref_request", x => x.RotationPrefRequestId);
                    table.ForeignKey(
                        name: "FK_rotation_pref_request_residents_resident_id",
                        column: x => x.resident_id,
                        principalTable: "residents",
                        principalColumn: "resident_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_rotation_pref_request_resident_id",
                table: "rotation_pref_request",
                column: "resident_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rotation_pref_request");
        }
    }
}