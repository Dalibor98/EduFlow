using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduFlow.Migrations
{
    /// <inheritdoc />
    public partial class AdminCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[] { 1, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "admin@test.com", "Dalibor Naspalic", "$2a$11$B/.urO6nJZAsSML3OCX5QOoj0TG1Kf2HMBgC6NExTb2DJwUdEl8tG", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
