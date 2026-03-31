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
#pragma warning disable CS0618 // Type or member is obsolete
                    { SeedingData.InpatientPsy, true, true, true, true, false, 15, "Inpatient Psy" },
                    { SeedingData.ImInpatient, false, false, false, false, false, 7, "ImInpatient" },
                    { SeedingData.NightFloat, true, true, true, true, false, 7, "NightFloat" },
                    { SeedingData.PhpAndIop, true, true, true, true, false, 7, "PHPandIOP" },
                    { SeedingData.Iop, true, true, true, true, false, 8, "IOP" },
                    { SeedingData.ImOutpatient, false, true, true, true, false, 7, "ImOutpatient" },
                    { SeedingData.Addiction, true, true, true, true, false, 15, "Addiction" },
                    { SeedingData.Hpc, true, true, true, true, false, 8, "HPC" },
                    { SeedingData.Clc, true, true, true, true, false, 8, "CLC" },
                    { SeedingData.CommunityPsy, true, true, true, true, false, 15, "Community Psy" },
                    { SeedingData.Cap, true, true, true, true, false, 7, "CAP" },
                    { SeedingData.EmergencyMed, false, false, false, false, false, 7, "EmergencyMed" },
                    { SeedingData.Sum, true, true, true, true, false, 8, "Sum" },
                    { SeedingData.Va, true, true, true, true, false, 8, "VA" },
                    { SeedingData.PsyConsults, true, true, true, true, false, 15, "Psy Consults" },
                    { SeedingData.Neurology, false, true, true, true, false, 7, "Neurology" },
                    { SeedingData.Float, true, true, true, true, false, 7, "Float" },
                    { SeedingData.Nfetc, true, true, true, true, false, 8, "NFETC" },
                    { SeedingData.Forensic, true, true, true, true, false, 15, "Forensic" },
                    { SeedingData.Chief, true, true, true, true, true, 8, "Chief" },
                    { SeedingData.Tms, true, true, true, true, false, 8, "TMS" },
                    { SeedingData.Unassigned, true, true, true, true, false, 15, "Unassigned" },
                    { SeedingData.Geriatric, true, true, true, true, false, 7, "Geriatric" }
#pragma warning restore CS0618 // Type or member is obsolete
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rotation_type");
        }
    }
}