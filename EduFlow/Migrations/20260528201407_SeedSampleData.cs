using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduFlow.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "professor@test.com", "Sample Professor", "$2a$11$45oGuVIWkiHCMrO681plx.0gmABwOFPec1mhU/2Gjtidy6z37Wqaa", "Professor" },
                    { 3, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "student@test.com", "Sample Student", "$2a$11$r6mINeiM1wLrDd8rDuVPEudiV32my8lYrqnTZkNukaUisVMVo.5du", "Student" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CreatedAt", "Description", "ProfessorId", "Title" },
                values: new object[] { 1, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "A beginner friendly course covering the fundamentals of building REST APIs with ASP.NET Core 8.", 2, "Introduction to ASP.NET Core" });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CourseId", "EnrolledAt", "UserId" },
                values: new object[] { 1, 1, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), 3 });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CourseId", "CreatedAt", "Description", "Title" },
                values: new object[] { 1, 1, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Learn how controllers, routing, and model binding work together in ASP.NET Core.", "Getting Started with Controllers" });

            migrationBuilder.InsertData(
                table: "Assignments",
                columns: new[] { "Id", "Description", "DueAt", "MaxScore", "ModuleId", "Title" },
                values: new object[] { 1, "Create a simple controller with two GET endpoints and submit your code.", new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 100, 1, "Build Your First Controller" });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "CreatedAt", "Description", "ModuleId", "Title" },
                values: new object[] { 1, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "A short quiz to test your understanding of controller fundamentals.", 1, "Controllers Basics Quiz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
