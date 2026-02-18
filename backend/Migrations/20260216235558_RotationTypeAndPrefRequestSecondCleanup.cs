using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDemo.Migrations
{
    public partial class RotationTypeAndPrefRequestSecondCleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_residents_resident_id",
                table: "rotation_pref_request");

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

            migrationBuilder.RenameColumn(
                name: "rotation_name",
                table: "rotation_type",
                newName: "RotationName");

            migrationBuilder.RenameColumn(
                name: "pgy_year_flags",
                table: "rotation_type",
                newName: "PgyYearFlags");

            migrationBuilder.RenameColumn(
                name: "is_chief_rotation",
                table: "rotation_type",
                newName: "IsChiefRotation");

            migrationBuilder.RenameColumn(
                name: "does_training_short_call",
                table: "rotation_type",
                newName: "DoesTrainingShortCall");

            migrationBuilder.RenameColumn(
                name: "does_training_long_call",
                table: "rotation_type",
                newName: "DoesTrainingLongCall");

            migrationBuilder.RenameColumn(
                name: "does_short_call",
                table: "rotation_type",
                newName: "DoesShortCall");

            migrationBuilder.RenameColumn(
                name: "does_long_call",
                table: "rotation_type",
                newName: "DoesLongCall");

            migrationBuilder.RenameColumn(
                name: "rotation_type_id",
                table: "rotation_type",
                newName: "RotationTypeId");

            migrationBuilder.RenameColumn(
                name: "third_priority_id",
                table: "rotation_pref_request",
                newName: "ThirdPriorityId");

            migrationBuilder.RenameColumn(
                name: "third_avoid_id",
                table: "rotation_pref_request",
                newName: "ThirdAvoidId");

            migrationBuilder.RenameColumn(
                name: "third_alternative_id",
                table: "rotation_pref_request",
                newName: "ThirdAlternativeId");

            migrationBuilder.RenameColumn(
                name: "sixth_priority_id",
                table: "rotation_pref_request",
                newName: "SixthPriorityId");

            migrationBuilder.RenameColumn(
                name: "seventh_priority_id",
                table: "rotation_pref_request",
                newName: "SeventhPriorityId");

            migrationBuilder.RenameColumn(
                name: "second_priority_id",
                table: "rotation_pref_request",
                newName: "SecondPriorityId");

            migrationBuilder.RenameColumn(
                name: "second_avoid_id",
                table: "rotation_pref_request",
                newName: "SecondAvoidId");

            migrationBuilder.RenameColumn(
                name: "second_alternative_id",
                table: "rotation_pref_request",
                newName: "SecondAlternativeId");

            migrationBuilder.RenameColumn(
                name: "resident_id",
                table: "rotation_pref_request",
                newName: "ResidentId");

            migrationBuilder.RenameColumn(
                name: "fourth_priority_id",
                table: "rotation_pref_request",
                newName: "FourthPriorityId");

            migrationBuilder.RenameColumn(
                name: "first_priority_id",
                table: "rotation_pref_request",
                newName: "FirstPriorityId");

            migrationBuilder.RenameColumn(
                name: "first_avoid_id",
                table: "rotation_pref_request",
                newName: "FirstAvoidId");

            migrationBuilder.RenameColumn(
                name: "first_alternative_id",
                table: "rotation_pref_request",
                newName: "FirstAlternativeId");

            migrationBuilder.RenameColumn(
                name: "fifth_priority_id",
                table: "rotation_pref_request",
                newName: "FifthPriorityId");

            migrationBuilder.RenameColumn(
                name: "eighth_priority_id",
                table: "rotation_pref_request",
                newName: "EighthPriorityId");

            migrationBuilder.RenameColumn(
                name: "additional_notes",
                table: "rotation_pref_request",
                newName: "AdditionalNotes");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_third_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_ThirdPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_third_avoid_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_ThirdAvoidId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_third_alternative_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_ThirdAlternativeId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_sixth_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_SixthPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_seventh_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_SeventhPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_second_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_SecondPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_second_avoid_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_SecondAvoidId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_second_alternative_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_SecondAlternativeId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_resident_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_ResidentId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_fourth_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_FourthPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_first_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_FirstPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_first_avoid_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_FirstAvoidId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_first_alternative_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_FirstAlternativeId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_fifth_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_FifthPriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_eighth_priority_id",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_EighthPriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_residents_ResidentId",
                table: "rotation_pref_request",
                column: "ResidentId",
                principalTable: "residents",
                principalColumn: "resident_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_EighthPriorityId",
                table: "rotation_pref_request",
                column: "EighthPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FifthPriorityId",
                table: "rotation_pref_request",
                column: "FifthPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstAlternativeId",
                table: "rotation_pref_request",
                column: "FirstAlternativeId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstAvoidId",
                table: "rotation_pref_request",
                column: "FirstAvoidId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstPriorityId",
                table: "rotation_pref_request",
                column: "FirstPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FourthPriorityId",
                table: "rotation_pref_request",
                column: "FourthPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondAlternativeId",
                table: "rotation_pref_request",
                column: "SecondAlternativeId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondAvoidId",
                table: "rotation_pref_request",
                column: "SecondAvoidId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondPriorityId",
                table: "rotation_pref_request",
                column: "SecondPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SeventhPriorityId",
                table: "rotation_pref_request",
                column: "SeventhPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SixthPriorityId",
                table: "rotation_pref_request",
                column: "SixthPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdAlternativeId",
                table: "rotation_pref_request",
                column: "ThirdAlternativeId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdAvoidId",
                table: "rotation_pref_request",
                column: "ThirdAvoidId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdPriorityId",
                table: "rotation_pref_request",
                column: "ThirdPriorityId",
                principalTable: "rotation_type",
                principalColumn: "RotationTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_residents_ResidentId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_EighthPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FifthPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstAlternativeId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstAvoidId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FirstPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_FourthPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondAlternativeId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondAvoidId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SecondPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SeventhPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_SixthPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdAlternativeId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdAvoidId",
                table: "rotation_pref_request");

            migrationBuilder.DropForeignKey(
                name: "FK_rotation_pref_request_rotation_type_ThirdPriorityId",
                table: "rotation_pref_request");

            migrationBuilder.RenameColumn(
                name: "RotationName",
                table: "rotation_type",
                newName: "rotation_name");

            migrationBuilder.RenameColumn(
                name: "PgyYearFlags",
                table: "rotation_type",
                newName: "pgy_year_flags");

            migrationBuilder.RenameColumn(
                name: "IsChiefRotation",
                table: "rotation_type",
                newName: "is_chief_rotation");

            migrationBuilder.RenameColumn(
                name: "DoesTrainingShortCall",
                table: "rotation_type",
                newName: "does_training_short_call");

            migrationBuilder.RenameColumn(
                name: "DoesTrainingLongCall",
                table: "rotation_type",
                newName: "does_training_long_call");

            migrationBuilder.RenameColumn(
                name: "DoesShortCall",
                table: "rotation_type",
                newName: "does_short_call");

            migrationBuilder.RenameColumn(
                name: "DoesLongCall",
                table: "rotation_type",
                newName: "does_long_call");

            migrationBuilder.RenameColumn(
                name: "RotationTypeId",
                table: "rotation_type",
                newName: "rotation_type_id");

            migrationBuilder.RenameColumn(
                name: "ThirdPriorityId",
                table: "rotation_pref_request",
                newName: "third_priority_id");

            migrationBuilder.RenameColumn(
                name: "ThirdAvoidId",
                table: "rotation_pref_request",
                newName: "third_avoid_id");

            migrationBuilder.RenameColumn(
                name: "ThirdAlternativeId",
                table: "rotation_pref_request",
                newName: "third_alternative_id");

            migrationBuilder.RenameColumn(
                name: "SixthPriorityId",
                table: "rotation_pref_request",
                newName: "sixth_priority_id");

            migrationBuilder.RenameColumn(
                name: "SeventhPriorityId",
                table: "rotation_pref_request",
                newName: "seventh_priority_id");

            migrationBuilder.RenameColumn(
                name: "SecondPriorityId",
                table: "rotation_pref_request",
                newName: "second_priority_id");

            migrationBuilder.RenameColumn(
                name: "SecondAvoidId",
                table: "rotation_pref_request",
                newName: "second_avoid_id");

            migrationBuilder.RenameColumn(
                name: "SecondAlternativeId",
                table: "rotation_pref_request",
                newName: "second_alternative_id");

            migrationBuilder.RenameColumn(
                name: "ResidentId",
                table: "rotation_pref_request",
                newName: "resident_id");

            migrationBuilder.RenameColumn(
                name: "FourthPriorityId",
                table: "rotation_pref_request",
                newName: "fourth_priority_id");

            migrationBuilder.RenameColumn(
                name: "FirstPriorityId",
                table: "rotation_pref_request",
                newName: "first_priority_id");

            migrationBuilder.RenameColumn(
                name: "FirstAvoidId",
                table: "rotation_pref_request",
                newName: "first_avoid_id");

            migrationBuilder.RenameColumn(
                name: "FirstAlternativeId",
                table: "rotation_pref_request",
                newName: "first_alternative_id");

            migrationBuilder.RenameColumn(
                name: "FifthPriorityId",
                table: "rotation_pref_request",
                newName: "fifth_priority_id");

            migrationBuilder.RenameColumn(
                name: "EighthPriorityId",
                table: "rotation_pref_request",
                newName: "eighth_priority_id");

            migrationBuilder.RenameColumn(
                name: "AdditionalNotes",
                table: "rotation_pref_request",
                newName: "additional_notes");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_ThirdPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_third_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_ThirdAvoidId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_third_avoid_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_ThirdAlternativeId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_third_alternative_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_SixthPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_sixth_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_SeventhPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_seventh_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_SecondPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_second_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_SecondAvoidId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_second_avoid_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_SecondAlternativeId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_second_alternative_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_ResidentId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_resident_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_FourthPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_fourth_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_FirstPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_first_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_FirstAvoidId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_first_avoid_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_FirstAlternativeId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_first_alternative_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_FifthPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_fifth_priority_id");

            migrationBuilder.RenameIndex(
                name: "IX_rotation_pref_request_EighthPriorityId",
                table: "rotation_pref_request",
                newName: "IX_rotation_pref_request_eighth_priority_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rotation_pref_request_residents_resident_id",
                table: "rotation_pref_request",
                column: "resident_id",
                principalTable: "residents",
                principalColumn: "resident_id",
                onDelete: ReferentialAction.Cascade);

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
    }
}