using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFieldOfStudyFromSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specializations_FieldsOfStudy_FieldOfStudyId",
                table: "Specializations");

            migrationBuilder.DropIndex(
                name: "IX_Specializations_FieldOfStudyId",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "FieldOfStudyId",
                table: "Specializations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldOfStudyId",
                table: "Specializations",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_FieldOfStudyId",
                table: "Specializations",
                column: "FieldOfStudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specializations_FieldsOfStudy_FieldOfStudyId",
                table: "Specializations",
                column: "FieldOfStudyId",
                principalTable: "FieldsOfStudy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
