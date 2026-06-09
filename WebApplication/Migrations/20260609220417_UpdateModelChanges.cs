using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AcademicTitle",
                table: "Teachers",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AcademicTitle",
                table: "Teachers",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
