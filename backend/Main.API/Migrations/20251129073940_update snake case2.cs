using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.API.Migrations
{
    /// <inheritdoc />
    public partial class updatesnakecase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ai_daily_usage_users_UserId",
                table: "ai_daily_usage");

            migrationBuilder.DropForeignKey(
                name: "FK_ai_minute_usage_users_UserId",
                table: "ai_minute_usage");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_embeddings_issues_IssueId",
                table: "issue_embeddings");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "notifications",
                newName: "payload");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "notifications",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Embedding",
                table: "issue_embeddings",
                newName: "embedding");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "issue_embeddings",
                newName: "issue_id");

            migrationBuilder.RenameIndex(
                name: "IX_issue_embeddings_IssueId",
                table: "issue_embeddings",
                newName: "IX_issue_embeddings_issue_id");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "ai_minute_usage",
                newName: "count");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ai_minute_usage",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "MinuteBucket",
                table: "ai_minute_usage",
                newName: "minute_bucket");

            migrationBuilder.RenameIndex(
                name: "IX_ai_minute_usage_UserId_MinuteBucket",
                table: "ai_minute_usage",
                newName: "IX_ai_minute_usage_user_id_minute_bucket");

            migrationBuilder.RenameIndex(
                name: "IX_ai_minute_usage_UserId",
                table: "ai_minute_usage",
                newName: "IX_ai_minute_usage_user_id");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "ai_daily_usage",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "ai_daily_usage",
                newName: "count");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ai_daily_usage",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_ai_daily_usage_UserId_Date",
                table: "ai_daily_usage",
                newName: "IX_ai_daily_usage_user_id_date");

            migrationBuilder.RenameIndex(
                name: "IX_ai_daily_usage_UserId",
                table: "ai_daily_usage",
                newName: "IX_ai_daily_usage_user_id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ai_daily_usage_users_user_id",
                table: "ai_daily_usage",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ai_minute_usage_users_user_id",
                table: "ai_minute_usage",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_embeddings_issues_issue_id",
                table: "issue_embeddings",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ai_daily_usage_users_user_id",
                table: "ai_daily_usage");

            migrationBuilder.DropForeignKey(
                name: "FK_ai_minute_usage_users_user_id",
                table: "ai_minute_usage");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_embeddings_issues_issue_id",
                table: "issue_embeddings");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "payload",
                table: "notifications",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "notifications",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "embedding",
                table: "issue_embeddings",
                newName: "Embedding");

            migrationBuilder.RenameColumn(
                name: "issue_id",
                table: "issue_embeddings",
                newName: "IssueId");

            migrationBuilder.RenameIndex(
                name: "IX_issue_embeddings_issue_id",
                table: "issue_embeddings",
                newName: "IX_issue_embeddings_IssueId");

            migrationBuilder.RenameColumn(
                name: "count",
                table: "ai_minute_usage",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ai_minute_usage",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "minute_bucket",
                table: "ai_minute_usage",
                newName: "MinuteBucket");

            migrationBuilder.RenameIndex(
                name: "IX_ai_minute_usage_user_id_minute_bucket",
                table: "ai_minute_usage",
                newName: "IX_ai_minute_usage_UserId_MinuteBucket");

            migrationBuilder.RenameIndex(
                name: "IX_ai_minute_usage_user_id",
                table: "ai_minute_usage",
                newName: "IX_ai_minute_usage_UserId");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "ai_daily_usage",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "count",
                table: "ai_daily_usage",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ai_daily_usage",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ai_daily_usage_user_id_date",
                table: "ai_daily_usage",
                newName: "IX_ai_daily_usage_UserId_Date");

            migrationBuilder.RenameIndex(
                name: "IX_ai_daily_usage_user_id",
                table: "ai_daily_usage",
                newName: "IX_ai_daily_usage_UserId");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 12, 29, 36, 215, DateTimeKind.Unspecified).AddTicks(5308), "$2a$11$rdyGZl3e/WK3uO9QbaUFX.R2LzTL23oeNx809kOYdfz6ZoutorAMu" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 3L,
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2025, 11, 29, 12, 29, 36, 294, DateTimeKind.Unspecified).AddTicks(3044), "$2a$11$04n0ya3EXL4RNEzmp8T3y.5xuOXxLMiS.lkXrL6MRZ0lheNLCwCfW" });

            migrationBuilder.AddForeignKey(
                name: "FK_ai_daily_usage_users_UserId",
                table: "ai_daily_usage",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ai_minute_usage_users_UserId",
                table: "ai_minute_usage",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_embeddings_issues_IssueId",
                table: "issue_embeddings",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
