using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClassType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_ClassTypes_ClassTypeId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Timetables_ClassTypes_ClassTypeId",
                table: "Timetables");

            migrationBuilder.DropTable(
                name: "ClassTypes");

            migrationBuilder.DropIndex(
                name: "IX_Timetables_ClassTypeId",
                table: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ClassTypeId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ClassTypeId",
                table: "Timetables");

            migrationBuilder.DropColumn(
                name: "ClassTypeId",
                table: "Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassTypeId",
                table: "Timetables",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassTypeId",
                table: "Groups",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClassTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Abbreviation = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_ClassTypeId",
                table: "Timetables",
                column: "ClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ClassTypeId",
                table: "Groups",
                column: "ClassTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_ClassTypes_ClassTypeId",
                table: "Groups",
                column: "ClassTypeId",
                principalTable: "ClassTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timetables_ClassTypes_ClassTypeId",
                table: "Timetables",
                column: "ClassTypeId",
                principalTable: "ClassTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
