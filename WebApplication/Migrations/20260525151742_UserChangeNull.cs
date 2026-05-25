using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class UserChangeNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "BOOLEAN",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN",
                oldNullable: true);
        }
    }
}
