using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.API.Migrations
{
    /// <inheritdoc />
    public partial class updatesubtask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "assignee_id",
                table: "issue_subtasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 15, 53, 38, 191, DateTimeKind.Unspecified).AddTicks(7178), "$2a$11$R5X1ZcYJ3chPx4e8jHQgW.aLqOQxAfJaorDHa97P9UVz8GYx55/a6" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 3L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 15, 53, 38, 270, DateTimeKind.Unspecified).AddTicks(9479), "$2a$11$aykNcrl0JKEbOwUD48MW9u7lIua/BSY1HV17VB5itdWzm6Z0o7u.y" });

            migrationBuilder.CreateIndex(
                name: "IX_issue_subtasks_assignee_id",
                table: "issue_subtasks",
                column: "assignee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_issue_subtasks_users_assignee_id",
                table: "issue_subtasks",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_issue_subtasks_users_assignee_id",
                table: "issue_subtasks");

            migrationBuilder.DropIndex(
                name: "IX_issue_subtasks_assignee_id",
                table: "issue_subtasks");

            migrationBuilder.DropColumn(
                name: "assignee_id",
                table: "issue_subtasks");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 14, 39, 40, 405, DateTimeKind.Unspecified).AddTicks(470), "$2a$11$x2JTJNN8OTUFKAz66mPAMuwSaWnsM.WwwPtZVxwSfe.6IQcxCFhMu" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 3L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 14, 39, 40, 486, DateTimeKind.Unspecified).AddTicks(1258), "$2a$11$d4cYtxrHEq93IgLX8IUNtuI8OkQGi7.EftNaF59GGqe/kbwcBRulC" });
        }
    }
}
