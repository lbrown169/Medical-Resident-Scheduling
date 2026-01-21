using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AlterDatabase()
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "admins",
            //     columns: table => new
            //     {
            //         admin_id = table.Column<string>(type: "varchar(255)", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         first_name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         last_name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         email = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         password = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         phone_num = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_admins", x => x.admin_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "announcements",
            //     columns: table => new
            //     {
            //         announcement_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         author_id = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         message = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_announcements", x => x.announcement_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "blackouts",
            //     columns: table => new
            //     {
            //         blackout_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         resident_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_blackouts", x => x.blackout_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "invitations",
            //     columns: table => new
            //     {
            //         token = table.Column<string>(type: "varchar(255)", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         resident_id = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         expires = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         used = table.Column<bool>(type: "tinyint(1)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_invitations", x => x.token);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "residents",
            //     columns: table => new
            //     {
            //         resident_id = table.Column<string>(type: "varchar(255)", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         first_name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         last_name = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         graduate_yr = table.Column<int>(type: "int", nullable: false),
            //         email = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         password = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         phone_num = table.Column<string>(type: "longtext", nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         weekly_hours = table.Column<int>(type: "int", nullable: false),
            //         total_hours = table.Column<int>(type: "int", nullable: false),
            //         bi_yearly_hours = table.Column<int>(type: "int", nullable: false),
            //         hospital_role_profile = table.Column<int>(type: "int", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_residents", x => x.resident_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "rotations",
            //     columns: table => new
            //     {
            //         rotation_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         resident_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         month = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         rotation = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_rotations", x => x.rotation_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "schedules",
            //     columns: table => new
            //     {
            //         schedule_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         status = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_schedules", x => x.schedule_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "swap_requests",
            //     columns: table => new
            //     {
            //         idswap_requests = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         schedule_swap_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         requester_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         requestee_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         requester_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         requestee_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         status = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //         updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
            //         details = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_swap_requests", x => x.idswap_requests);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "vacations",
            //     columns: table => new
            //     {
            //         vacation_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         resident_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         reason = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         status = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         details = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         groupId = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_vacations", x => x.vacation_id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "dates",
            //     columns: table => new
            //     {
            //         date_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         schedule_id = table.Column<byte[]>(type: "binary(16)", nullable: false),
            //         resident_id = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         call_type = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         hours = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_dates", x => x.date_id);
            //         table.ForeignKey(
            //             name: "FK_dates_residents_resident_id",
            //             column: x => x.resident_id,
            //             principalTable: "residents",
            //             principalColumn: "resident_id",
            //             onDelete: ReferentialAction.Cascade);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateIndex(
            //     name: "IX_dates_resident_id",
            //     table: "dates",
            //     column: "resident_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "admins");

            // migrationBuilder.DropTable(
            //     name: "announcements");

            // migrationBuilder.DropTable(
            //     name: "blackouts");

            // migrationBuilder.DropTable(
            //     name: "dates");

            // migrationBuilder.DropTable(
            //     name: "invitations");

            // migrationBuilder.DropTable(
            //     name: "rotations");

            // migrationBuilder.DropTable(
            //     name: "schedules");

            // migrationBuilder.DropTable(
            //     name: "swap_requests");

            // migrationBuilder.DropTable(
            //     name: "vacations");

            // migrationBuilder.DropTable(
            //     name: "residents");
        }
    }
}
