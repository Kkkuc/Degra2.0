using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class SpecialisationStructureChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Specializations_SpecializationId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "SpecializationId",
                table: "Groups",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddColumn<int>(
                name: "FieldOfStudyId",
                table: "Groups",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FieldOfStudyId",
                table: "Groups",
                column: "FieldOfStudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_FieldsOfStudy_FieldOfStudyId",
                table: "Groups",
                column: "FieldOfStudyId",
                principalTable: "FieldsOfStudy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Specializations_SpecializationId",
                table: "Groups",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_FieldsOfStudy_FieldOfStudyId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Specializations_SpecializationId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_FieldOfStudyId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "FieldOfStudyId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "SpecializationId",
                table: "Groups",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Specializations_SpecializationId",
                table: "Groups",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
