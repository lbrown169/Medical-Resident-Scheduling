using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class AddForeignKeyFromResidentsToVacation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "resident_id_vacations",
                table: "vacations",
                column: "resident_id",
                principalTable: "residents",
                principalColumn: "resident_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "resident_id_vacations",
                table: "vacations");
        }
    }
}
