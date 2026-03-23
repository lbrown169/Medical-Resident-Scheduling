using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationTypeTablePopulateAndSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rotation_type",
                columns: table => new
                {
                    rotation_type_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
                    rotation_name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    does_long_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    does_short_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    does_training_long_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    does_training_short_call = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_chief_rotation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    pgy_year_flags = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rotation_type", x => x.rotation_type_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.InsertData(
                table: "rotation_type",
                columns: new[] { "rotation_type_id", "does_long_call", "does_short_call", "does_training_long_call", "does_training_short_call", "is_chief_rotation", "pgy_year_flags", "rotation_name" },
                values: new object[,]
                {
                    { new byte[] { 43, 16, 19, 7, 193, 3, 98, 74, 191, 216, 76, 4, 54, 154, 199, 196 }, true, true, true, true, false, 15, "Inpatient Psy" },
                    { new byte[] { 127, 107, 64, 16, 183, 28, 13, 77, 177, 78, 121, 131, 164, 140, 28, 232 }, false, false, false, false, false, 7, "ImInpatient" },
                    { new byte[] { 198, 128, 90, 38, 155, 98, 143, 70, 131, 234, 116, 101, 192, 246, 51, 195 }, true, true, true, true, false, 7, "NightFloat" },
                    { new byte[] { 165, 18, 107, 68, 24, 126, 253, 72, 138, 187, 32, 211, 154, 90, 163, 83 }, true, true, true, true, false, 7, "PHPandIOP" },
                    { new byte[] { 191, 177, 71, 69, 124, 87, 217, 74, 130, 70, 127, 8, 94, 217, 87, 249 }, true, true, true, true, false, 8, "IOP" },
                    { new byte[] { 88, 181, 70, 110, 90, 85, 4, 68, 129, 85, 19, 29, 197, 159, 164, 48 }, false, true, true, true, false, 7, "ImOutpatient" },
                    { new byte[] { 215, 39, 22, 119, 159, 186, 73, 69, 137, 138, 66, 103, 253, 20, 228, 58 }, true, true, true, true, false, 15, "Addiction" },
                    { new byte[] { 65, 201, 67, 126, 192, 225, 88, 76, 143, 65, 83, 94, 26, 26, 130, 219 }, true, true, true, true, false, 8, "HPC" },
                    { new byte[] { 41, 63, 243, 128, 169, 207, 66, 68, 180, 88, 117, 40, 90, 217, 147, 196 }, true, true, true, true, false, 8, "CLC" },
                    { new byte[] { 110, 22, 50, 137, 36, 220, 60, 79, 136, 215, 63, 185, 238, 126, 22, 26 }, true, true, true, true, false, 15, "Community Psy" },
                    { new byte[] { 209, 143, 230, 137, 204, 40, 127, 77, 176, 253, 32, 254, 216, 57, 98, 49 }, true, true, true, true, false, 7, "CAP" },
                    { new byte[] { 81, 121, 28, 144, 20, 87, 143, 72, 151, 49, 99, 107, 225, 251, 106, 236 }, false, false, false, false, false, 7, "EmergencyMed" },
                    { new byte[] { 129, 242, 99, 144, 25, 121, 55, 73, 180, 107, 234, 4, 20, 112, 24, 202 }, true, true, true, true, false, 8, "Sum" },
                    { new byte[] { 217, 68, 77, 152, 241, 47, 25, 73, 168, 149, 204, 134, 90, 158, 72, 114 }, true, true, true, true, false, 8, "VA" },
                    { new byte[] { 215, 87, 101, 160, 153, 16, 236, 66, 156, 238, 237, 62, 236, 98, 19, 171 }, true, true, true, true, false, 15, "Psy Consults" },
                    { new byte[] { 219, 221, 77, 165, 69, 2, 130, 69, 135, 54, 87, 227, 13, 138, 12, 23 }, false, true, true, true, false, 7, "Neurology" },
                    { new byte[] { 146, 142, 124, 165, 250, 119, 15, 65, 190, 84, 145, 38, 136, 28, 53, 20 }, true, true, true, true, false, 7, "Float" },
                    { new byte[] { 50, 124, 227, 176, 25, 121, 82, 77, 191, 32, 242, 114, 201, 138, 78, 131 }, true, true, true, true, false, 8, "NFETC" },
                    { new byte[] { 44, 1, 68, 181, 224, 204, 217, 73, 178, 214, 31, 23, 6, 141, 183, 210 }, true, true, true, true, false, 15, "Forensic" },
                    { new byte[] { 78, 40, 245, 224, 213, 0, 234, 76, 186, 46, 163, 113, 141, 91, 112, 196 }, true, true, true, true, true, 8, "Chief" },
                    { new byte[] { 16, 45, 246, 231, 218, 87, 73, 71, 156, 16, 11, 78, 197, 56, 160, 71 }, true, true, true, true, false, 8, "TMS" },
                    { new byte[] { 163, 198, 184, 245, 23, 100, 68, 75, 189, 163, 202, 94, 123, 238, 8, 109 }, true, true, true, true, false, 15, "Unassigned" },
                    { new byte[] { 234, 143, 171, 248, 255, 175, 221, 70, 174, 199, 66, 61, 121, 28, 232, 209 }, true, true, true, true, false, 7, "Geriatric" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rotation_type");
        }
    }
}