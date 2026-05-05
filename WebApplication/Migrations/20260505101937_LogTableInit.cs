using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class LogTableInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TableName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Operation = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OldValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NewValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UserChanged = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
