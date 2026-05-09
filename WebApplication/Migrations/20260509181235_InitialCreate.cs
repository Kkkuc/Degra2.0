using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PermissionCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    StudentID = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Abbreviation = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AcademicTitle = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AcademicYearId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semesters_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    FacultyId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldsOfStudy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FacultyId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Degree = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Mode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsOfStudy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldsOfStudy_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PermissionId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Username = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StudentId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TeacherId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BuildingId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RoomNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    RoomType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SemesterId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FieldOfStudyId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SpecializationId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ClassType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_FieldsOfStudy_FieldOfStudyId",
                        column: x => x.FieldOfStudyId,
                        principalTable: "FieldsOfStudy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Groups_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    GroupId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => new { x.StudentId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_StudentGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroups_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Timetables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SubjectId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TeacherId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RoomId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    GroupId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClassType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DayOfWeek = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "INTERVAL DAY(8) TO SECOND(7)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "INTERVAL DAY(8) TO SECOND(7)", nullable: false),
                    WeekCycle = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timetables_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Timetables_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Timetables_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Timetables_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TimetableId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ChangeType = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NewRoomId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NewTeacherId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NewStartTime = table.Column<TimeSpan>(type: "INTERVAL DAY(8) TO SECOND(7)", nullable: true),
                    NewEndTime = table.Column<TimeSpan>(type: "INTERVAL DAY(8) TO SECOND(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleChanges_Rooms_NewRoomId",
                        column: x => x.NewRoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduleChanges_Teachers_NewTeacherId",
                        column: x => x.NewTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduleChanges_Timetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "Timetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_FacultyId",
                table: "Buildings",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsOfStudy_FacultyId",
                table: "FieldsOfStudy",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FieldOfStudyId",
                table: "Groups",
                column: "FieldOfStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SemesterId",
                table: "Groups",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SpecializationId",
                table: "Groups",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleChanges_NewRoomId",
                table: "ScheduleChanges",
                column: "NewRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleChanges_NewTeacherId",
                table: "ScheduleChanges",
                column: "NewTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleChanges_TimetableId",
                table: "ScheduleChanges",
                column: "TimetableId");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_AcademicYearId",
                table: "Semesters",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_GroupId",
                table: "StudentGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_GroupId",
                table: "Timetables",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_RoomId",
                table: "Timetables",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_SubjectId",
                table: "Timetables",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_TeacherId",
                table: "Timetables",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentId",
                table: "Users",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeacherId",
                table: "Users",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "ScheduleChanges");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Timetables");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "FieldsOfStudy");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Faculties");
        }
    }
}
