using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class BuildingAddressChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Buildings");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Buildings",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Buildings",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Buildings",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Buildings",
                type: "NVARCHAR2(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Buildings");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Buildings",
                type: "NVARCHAR2(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
