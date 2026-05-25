using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RepairFacultyConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldsOfStudy_Faculties_DepartmentId",
                table: "FieldsOfStudy");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "FieldsOfStudy",
                newName: "FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldsOfStudy_DepartmentId",
                table: "FieldsOfStudy",
                newName: "IX_FieldsOfStudy_FacultyId");

            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "Buildings",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_FacultyId",
                table: "Buildings",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Faculties_FacultyId",
                table: "Buildings",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsOfStudy_Faculties_FacultyId",
                table: "FieldsOfStudy",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Faculties_FacultyId",
                table: "Buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldsOfStudy_Faculties_FacultyId",
                table: "FieldsOfStudy");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_FacultyId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Buildings");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "FieldsOfStudy",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldsOfStudy_FacultyId",
                table: "FieldsOfStudy",
                newName: "IX_FieldsOfStudy_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsOfStudy_Faculties_DepartmentId",
                table: "FieldsOfStudy",
                column: "DepartmentId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
