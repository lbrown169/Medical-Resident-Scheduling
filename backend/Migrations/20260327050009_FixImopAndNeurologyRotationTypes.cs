using Microsoft.EntityFrameworkCore.Migrations;
#pragma warning disable CS0618 // Type or member is obsolete

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class FixImopAndNeurologyRotationTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "rotation_type",
                keyColumn: "RotationTypeId",
                keyValue: SeedingData.Neurology,
                columns: new[] { "DoesLongCall", "DoesShortCall" },
                values: new object[] { true, false }
            );

            migrationBuilder.UpdateData(
                table: "rotation_type",
                keyColumn: "RotationTypeId",
                keyValue: SeedingData.ImOutpatient,
                columns: new[] { "DoesLongCall", "DoesShortCall" },
                values: new object[] { true, false }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "rotation_type",
                keyColumn: "rotation_type_id",
                keyValue: SeedingData.Neurology,
                columns: new[] { "does_long_call", "does_short_call" },
                values: new object[] { false, true }
            );

            migrationBuilder.UpdateData(
                table: "rotation_type",
                keyColumn: "rotation_type_id",
                keyValue: SeedingData.ImOutpatient,
                columns: new[] { "does_long_call", "does_short_call" },
                values: new object[] { false, true }
            );
        }
    }
}