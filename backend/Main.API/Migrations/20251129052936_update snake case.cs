using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.API.Migrations
{
    /// <inheritdoc />
    public partial class updatesnakecase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_issues_IssueId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_projects_projects_ProjectId",
                table: "favorite_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_projects_users_UserId",
                table: "favorite_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_history_issues_IssueId",
                table: "issue_history");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_history_users_ActorId",
                table: "issue_history");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_labels_issues_IssueId",
                table: "issue_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_labels_project_labels_LabelId",
                table: "issue_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_statuses_projects_ProjectId",
                table: "issue_statuses");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_subtasks_issues_IssueId",
                table: "issue_subtasks");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_issue_statuses_StatusId",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_projects_ProjectId",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_users_AssigneeId",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_users_OwnerId",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_password_reset_tokens_users_UserId",
                table: "password_reset_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_project_labels_projects_ProjectId",
                table: "project_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_teams_TeamId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_OwnerId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_team_activity_logs_teams_TeamId",
                table: "team_activity_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_team_activity_logs_users_ActorId",
                table: "team_activity_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_teams_TeamId",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_CreatedBy",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_team_members_teams_TeamId",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_team_members_users_UserId",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_users_OwnerId",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "FK_user_auth_providers_users_UserId",
                table: "user_auth_providers");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "users",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "users",
                newName: "avatar");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserOrigin",
                table: "users",
                newName: "user_origin");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "users",
                newName: "google_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "user_auth_providers",
                newName: "provider");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user_auth_providers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_auth_providers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_auth_providers",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "user_auth_providers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProviderUserId",
                table: "user_auth_providers",
                newName: "provider_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_auth_providers",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_user_auth_providers_UserId",
                table: "user_auth_providers",
                newName: "IX_user_auth_providers_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_auth_providers_Provider_ProviderUserId",
                table: "user_auth_providers",
                newName: "IX_user_auth_providers_provider_provider_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "teams",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "teams",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "teams",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "teams",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "teams",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "teams",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_teams_OwnerId",
                table: "teams",
                newName: "IX_teams_owner_id");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "team_members",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "team_members",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "team_members",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "team_members",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "team_members",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "team_members",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_UserId",
                table: "team_members",
                newName: "IX_team_members_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_TeamId_UserId",
                table: "team_members",
                newName: "IX_team_members_team_id_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_TeamId",
                table: "team_members",
                newName: "IX_team_members_team_id");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "team_invites",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "team_invites",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "team_invites",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "team_invites",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "team_invites",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "team_invites",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "team_invites",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "team_invites",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AcceptedAt",
                table: "team_invites",
                newName: "accepted_at");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_Token",
                table: "team_invites",
                newName: "IX_team_invites_token");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_TeamId_Email",
                table: "team_invites",
                newName: "IX_team_invites_team_id_email");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_CreatedBy",
                table: "team_invites",
                newName: "IX_team_invites_created_by");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                table: "team_activity_logs",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "team_activity_logs",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "team_activity_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "team_activity_logs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "team_activity_logs",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "TargetType",
                table: "team_activity_logs",
                newName: "target_type");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "team_activity_logs",
                newName: "target_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "team_activity_logs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "team_activity_logs",
                newName: "actor_id");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "team_activity_logs",
                newName: "action_type");

            migrationBuilder.RenameIndex(
                name: "IX_team_activity_logs_TeamId_CreatedAt",
                table: "team_activity_logs",
                newName: "IX_team_activity_logs_team_id_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_team_activity_logs_ActorId",
                table: "team_activity_logs",
                newName: "IX_team_activity_logs_actor_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "projects",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "projects",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "projects",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "projects",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "projects",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "projects",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "projects",
                newName: "is_archived");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "projects",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "projects",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_projects_TeamId",
                table: "projects",
                newName: "IX_projects_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_projects_OwnerId",
                table: "projects",
                newName: "IX_projects_owner_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "project_labels",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "project_labels",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "project_labels",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "project_labels",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "project_labels",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "project_labels",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_project_labels_ProjectId_Name",
                table: "project_labels",
                newName: "IX_project_labels_project_id_name");

            migrationBuilder.RenameIndex(
                name: "IX_project_labels_ProjectId",
                table: "project_labels",
                newName: "IX_project_labels_project_id");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "password_reset_tokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "password_reset_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "password_reset_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UsedAt",
                table: "password_reset_tokens",
                newName: "used_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "password_reset_tokens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "password_reset_tokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "password_reset_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_tokens_Token",
                table: "password_reset_tokens",
                newName: "IX_password_reset_tokens_token");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_tokens_UserId",
                table: "password_reset_tokens",
                newName: "IX_password_reset_tokens_user_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "notifications",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "notifications",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "notifications",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notifications",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_UserId_IsRead",
                table: "notifications",
                newName: "IX_notifications_user_id_is_read");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_UserId_CreatedAt",
                table: "notifications",
                newName: "IX_notifications_user_id_created_at");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "issues",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "issues",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "issues",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "issues",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "issues",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "issues",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "issues",
                newName: "status_id");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "issues",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "issues",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "issues",
                newName: "due_date");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "issues",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "issues",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AssigneeId",
                table: "issues",
                newName: "assignee_id");

            migrationBuilder.RenameColumn(
                name: "AiSummaryGeneratedAt",
                table: "issues",
                newName: "ai_summary_generated_at");

            migrationBuilder.RenameColumn(
                name: "AiSummary",
                table: "issues",
                newName: "ai_summary");

            migrationBuilder.RenameColumn(
                name: "AiSuggestionGeneratedAt",
                table: "issues",
                newName: "ai_suggestion_generated_at");

            migrationBuilder.RenameColumn(
                name: "AiSuggestion",
                table: "issues",
                newName: "ai_suggestion");

            migrationBuilder.RenameColumn(
                name: "AiCommentSummaryGeneratedAt",
                table: "issues",
                newName: "ai_comment_summary_generated_at");

            migrationBuilder.RenameColumn(
                name: "AiCommentSummaryCommentCount",
                table: "issues",
                newName: "ai_comment_summary_comment_count");

            migrationBuilder.RenameColumn(
                name: "AiCommentSummary",
                table: "issues",
                newName: "ai_comment_summary");

            migrationBuilder.RenameIndex(
                name: "IX_issues_Priority",
                table: "issues",
                newName: "IX_issues_priority");

            migrationBuilder.RenameIndex(
                name: "IX_issues_StatusId",
                table: "issues",
                newName: "IX_issues_status_id");

            migrationBuilder.RenameIndex(
                name: "IX_issues_ProjectId_StatusId_Position",
                table: "issues",
                newName: "IX_issues_project_id_status_id_position");

            migrationBuilder.RenameIndex(
                name: "IX_issues_OwnerId",
                table: "issues",
                newName: "IX_issues_owner_id");

            migrationBuilder.RenameIndex(
                name: "IX_issues_DueDate",
                table: "issues",
                newName: "IX_issues_due_date");

            migrationBuilder.RenameIndex(
                name: "IX_issues_AssigneeId",
                table: "issues",
                newName: "IX_issues_assignee_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "issue_subtasks",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "issue_subtasks",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "issue_subtasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "issue_subtasks",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "issue_subtasks",
                newName: "issue_id");

            migrationBuilder.RenameColumn(
                name: "IsDone",
                table: "issue_subtasks",
                newName: "is_done");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "issue_subtasks",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_issue_subtasks_IssueId_Position",
                table: "issue_subtasks",
                newName: "IX_issue_subtasks_issue_id_position");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "issue_statuses",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "issue_statuses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "issue_statuses",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "issue_statuses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WipLimit",
                table: "issue_statuses",
                newName: "wip_limit");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "issue_statuses",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "issue_statuses",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "issue_statuses",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "issue_statuses",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "issue_statuses",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_issue_statuses_ProjectId_Position",
                table: "issue_statuses",
                newName: "IX_issue_statuses_project_id_position");

            migrationBuilder.RenameIndex(
                name: "IX_issue_statuses_ProjectId_Name",
                table: "issue_statuses",
                newName: "IX_issue_statuses_project_id_name");

            migrationBuilder.RenameColumn(
                name: "LabelId",
                table: "issue_labels",
                newName: "label_id");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "issue_labels",
                newName: "issue_id");

            migrationBuilder.RenameIndex(
                name: "IX_issue_labels_LabelId",
                table: "issue_labels",
                newName: "IX_issue_labels_label_id");

            migrationBuilder.RenameColumn(
                name: "Field",
                table: "issue_history",
                newName: "field");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "issue_history",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "issue_history",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "OldValue",
                table: "issue_history",
                newName: "old_value");

            migrationBuilder.RenameColumn(
                name: "NewValue",
                table: "issue_history",
                newName: "new_value");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "issue_history",
                newName: "issue_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "issue_history",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "issue_history",
                newName: "actor_id");

            migrationBuilder.RenameIndex(
                name: "IX_issue_history_IssueId_CreatedAt",
                table: "issue_history",
                newName: "IX_issue_history_issue_id_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_issue_history_ActorId",
                table: "issue_history",
                newName: "IX_issue_history_actor_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "issue_embeddings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "issue_embeddings",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "issue_embeddings",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "favorite_projects",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "favorite_projects",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "favorite_projects",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_favorite_projects_ProjectId",
                table: "favorite_projects",
                newName: "IX_favorite_projects_project_id");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "comments",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "comments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "comments",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "comments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "comments",
                newName: "issue_id");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "comments",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "comments",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_comments_UserId",
                table: "comments",
                newName: "IX_comments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_comments_IssueId_CreatedAt",
                table: "comments",
                newName: "IX_comments_issue_id_created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ai_minute_usage",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ai_minute_usage",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ai_minute_usage",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ai_daily_usage",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ai_daily_usage",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ai_daily_usage",
                newName: "created_at");

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
                name: "FK_comments_issues_issue_id",
                table: "comments",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_projects_projects_project_id",
                table: "favorite_projects",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_projects_users_user_id",
                table: "favorite_projects",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_history_issues_issue_id",
                table: "issue_history",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_history_users_actor_id",
                table: "issue_history",
                column: "actor_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_labels_issues_issue_id",
                table: "issue_labels",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_labels_project_labels_label_id",
                table: "issue_labels",
                column: "label_id",
                principalTable: "project_labels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_statuses_projects_project_id",
                table: "issue_statuses",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_subtasks_issues_issue_id",
                table: "issue_subtasks",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_issue_statuses_status_id",
                table: "issues",
                column: "status_id",
                principalTable: "issue_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_projects_project_id",
                table: "issues",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_users_assignee_id",
                table: "issues",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_issues_users_owner_id",
                table: "issues",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_password_reset_tokens_users_user_id",
                table: "password_reset_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_labels_projects_project_id",
                table: "project_labels",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_teams_team_id",
                table: "projects",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_owner_id",
                table: "projects",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_activity_logs_teams_team_id",
                table: "team_activity_logs",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_activity_logs_users_actor_id",
                table: "team_activity_logs",
                column: "actor_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_teams_team_id",
                table: "team_invites",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_created_by",
                table: "team_invites",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_teams_team_id",
                table: "team_members",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_users_user_id",
                table: "team_members",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_users_owner_id",
                table: "teams",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_auth_providers_users_user_id",
                table: "user_auth_providers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_issues_issue_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_user_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_projects_projects_project_id",
                table: "favorite_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_projects_users_user_id",
                table: "favorite_projects");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_history_issues_issue_id",
                table: "issue_history");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_history_users_actor_id",
                table: "issue_history");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_labels_issues_issue_id",
                table: "issue_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_labels_project_labels_label_id",
                table: "issue_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_statuses_projects_project_id",
                table: "issue_statuses");

            migrationBuilder.DropForeignKey(
                name: "FK_issue_subtasks_issues_issue_id",
                table: "issue_subtasks");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_issue_statuses_status_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_projects_project_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_users_assignee_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_issues_users_owner_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_password_reset_tokens_users_user_id",
                table: "password_reset_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_project_labels_projects_project_id",
                table: "project_labels");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_teams_team_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_owner_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_team_activity_logs_teams_team_id",
                table: "team_activity_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_team_activity_logs_users_actor_id",
                table: "team_activity_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_teams_team_id",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_created_by",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_team_members_teams_team_id",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_team_members_users_user_id",
                table: "team_members");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_users_owner_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "FK_user_auth_providers_users_user_id",
                table: "user_auth_providers");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "users",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "users",
                newName: "Avatar");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_origin",
                table: "users",
                newName: "UserOrigin");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "google_id",
                table: "users",
                newName: "GoogleId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "provider",
                table: "user_auth_providers",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "user_auth_providers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_auth_providers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_auth_providers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "user_auth_providers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "provider_user_id",
                table: "user_auth_providers",
                newName: "ProviderUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "user_auth_providers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_user_auth_providers_user_id",
                table: "user_auth_providers",
                newName: "IX_user_auth_providers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_auth_providers_provider_provider_user_id",
                table: "user_auth_providers",
                newName: "IX_user_auth_providers_Provider_ProviderUserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "teams",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "teams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "teams",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "teams",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "teams",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "teams",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_teams_owner_id",
                table: "teams",
                newName: "IX_teams_OwnerId");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "team_members",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "team_members",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "team_members",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "team_members",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "team_members",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "team_members",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_user_id",
                table: "team_members",
                newName: "IX_team_members_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_team_id_user_id",
                table: "team_members",
                newName: "IX_team_members_TeamId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_team_members_team_id",
                table: "team_members",
                newName: "IX_team_members_TeamId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "team_invites",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "team_invites",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "team_invites",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "team_invites",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "team_invites",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "team_invites",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "team_invites",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "team_invites",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "accepted_at",
                table: "team_invites",
                newName: "AcceptedAt");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_token",
                table: "team_invites",
                newName: "IX_team_invites_Token");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_team_id_email",
                table: "team_invites",
                newName: "IX_team_invites_TeamId_Email");

            migrationBuilder.RenameIndex(
                name: "IX_team_invites_created_by",
                table: "team_invites",
                newName: "IX_team_invites_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "metadata",
                table: "team_activity_logs",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "team_activity_logs",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "team_activity_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "team_activity_logs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "team_activity_logs",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "target_type",
                table: "team_activity_logs",
                newName: "TargetType");

            migrationBuilder.RenameColumn(
                name: "target_id",
                table: "team_activity_logs",
                newName: "TargetId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "team_activity_logs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "actor_id",
                table: "team_activity_logs",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "action_type",
                table: "team_activity_logs",
                newName: "ActionType");

            migrationBuilder.RenameIndex(
                name: "IX_team_activity_logs_team_id_created_at",
                table: "team_activity_logs",
                newName: "IX_team_activity_logs_TeamId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_team_activity_logs_actor_id",
                table: "team_activity_logs",
                newName: "IX_team_activity_logs_ActorId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "projects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "projects",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "projects",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "projects",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "is_archived",
                table: "projects",
                newName: "IsArchived");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "projects",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "projects",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_projects_team_id",
                table: "projects",
                newName: "IX_projects_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_owner_id",
                table: "projects",
                newName: "IX_projects_OwnerId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "project_labels",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "project_labels",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "project_labels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "project_labels",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "project_labels",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "project_labels",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_project_labels_project_id_name",
                table: "project_labels",
                newName: "IX_project_labels_ProjectId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_project_labels_project_id",
                table: "project_labels",
                newName: "IX_project_labels_ProjectId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "password_reset_tokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "password_reset_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "password_reset_tokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "used_at",
                table: "password_reset_tokens",
                newName: "UsedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "password_reset_tokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "password_reset_tokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "password_reset_tokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_tokens_token",
                table: "password_reset_tokens",
                newName: "IX_password_reset_tokens_Token");

            migrationBuilder.RenameIndex(
                name: "IX_password_reset_tokens_user_id",
                table: "password_reset_tokens",
                newName: "IX_password_reset_tokens_UserId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "notifications",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_user_id_is_read",
                table: "notifications",
                newName: "IX_notifications_UserId_IsRead");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_user_id_created_at",
                table: "notifications",
                newName: "IX_notifications_UserId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "issues",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "issues",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "issues",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "issues",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "issues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "issues",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "issues",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "issues",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "issues",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "due_date",
                table: "issues",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "issues",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "issues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "assignee_id",
                table: "issues",
                newName: "AssigneeId");

            migrationBuilder.RenameColumn(
                name: "ai_summary_generated_at",
                table: "issues",
                newName: "AiSummaryGeneratedAt");

            migrationBuilder.RenameColumn(
                name: "ai_summary",
                table: "issues",
                newName: "AiSummary");

            migrationBuilder.RenameColumn(
                name: "ai_suggestion_generated_at",
                table: "issues",
                newName: "AiSuggestionGeneratedAt");

            migrationBuilder.RenameColumn(
                name: "ai_suggestion",
                table: "issues",
                newName: "AiSuggestion");

            migrationBuilder.RenameColumn(
                name: "ai_comment_summary_generated_at",
                table: "issues",
                newName: "AiCommentSummaryGeneratedAt");

            migrationBuilder.RenameColumn(
                name: "ai_comment_summary_comment_count",
                table: "issues",
                newName: "AiCommentSummaryCommentCount");

            migrationBuilder.RenameColumn(
                name: "ai_comment_summary",
                table: "issues",
                newName: "AiCommentSummary");

            migrationBuilder.RenameIndex(
                name: "IX_issues_priority",
                table: "issues",
                newName: "IX_issues_Priority");

            migrationBuilder.RenameIndex(
                name: "IX_issues_status_id",
                table: "issues",
                newName: "IX_issues_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_issues_project_id_status_id_position",
                table: "issues",
                newName: "IX_issues_ProjectId_StatusId_Position");

            migrationBuilder.RenameIndex(
                name: "IX_issues_owner_id",
                table: "issues",
                newName: "IX_issues_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_issues_due_date",
                table: "issues",
                newName: "IX_issues_DueDate");

            migrationBuilder.RenameIndex(
                name: "IX_issues_assignee_id",
                table: "issues",
                newName: "IX_issues_AssigneeId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "issue_subtasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "issue_subtasks",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "issue_subtasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "issue_subtasks",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "issue_id",
                table: "issue_subtasks",
                newName: "IssueId");

            migrationBuilder.RenameColumn(
                name: "is_done",
                table: "issue_subtasks",
                newName: "IsDone");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "issue_subtasks",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_issue_subtasks_issue_id_position",
                table: "issue_subtasks",
                newName: "IX_issue_subtasks_IssueId_Position");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "issue_statuses",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "issue_statuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "issue_statuses",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "issue_statuses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wip_limit",
                table: "issue_statuses",
                newName: "WipLimit");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "issue_statuses",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "issue_statuses",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "issue_statuses",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "issue_statuses",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "issue_statuses",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_issue_statuses_project_id_position",
                table: "issue_statuses",
                newName: "IX_issue_statuses_ProjectId_Position");

            migrationBuilder.RenameIndex(
                name: "IX_issue_statuses_project_id_name",
                table: "issue_statuses",
                newName: "IX_issue_statuses_ProjectId_Name");

            migrationBuilder.RenameColumn(
                name: "label_id",
                table: "issue_labels",
                newName: "LabelId");

            migrationBuilder.RenameColumn(
                name: "issue_id",
                table: "issue_labels",
                newName: "IssueId");

            migrationBuilder.RenameIndex(
                name: "IX_issue_labels_label_id",
                table: "issue_labels",
                newName: "IX_issue_labels_LabelId");

            migrationBuilder.RenameColumn(
                name: "field",
                table: "issue_history",
                newName: "Field");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "issue_history",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "issue_history",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "old_value",
                table: "issue_history",
                newName: "OldValue");

            migrationBuilder.RenameColumn(
                name: "new_value",
                table: "issue_history",
                newName: "NewValue");

            migrationBuilder.RenameColumn(
                name: "issue_id",
                table: "issue_history",
                newName: "IssueId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "issue_history",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "actor_id",
                table: "issue_history",
                newName: "ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_issue_history_issue_id_created_at",
                table: "issue_history",
                newName: "IX_issue_history_IssueId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_issue_history_actor_id",
                table: "issue_history",
                newName: "IX_issue_history_ActorId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "issue_embeddings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "issue_embeddings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "issue_embeddings",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "favorite_projects",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "favorite_projects",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "favorite_projects",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_favorite_projects_project_id",
                table: "favorite_projects",
                newName: "IX_favorite_projects_ProjectId");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "comments",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "comments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "comments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "comments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "issue_id",
                table: "comments",
                newName: "IssueId");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "comments",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "comments",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_comments_user_id",
                table: "comments",
                newName: "IX_comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_issue_id_created_at",
                table: "comments",
                newName: "IX_comments_IssueId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ai_minute_usage",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ai_minute_usage",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ai_minute_usage",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ai_daily_usage",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ai_daily_usage",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ai_daily_usage",
                newName: "CreatedAt");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 11, 29, 12, 15, 46, 719, DateTimeKind.Unspecified).AddTicks(8269), "$2a$11$NF/uqhLFAUdWqmyPXPPLDeSaDqXhdVYFRdOTD5E74D5BTqaRsZZBi" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 11, 29, 12, 15, 46, 801, DateTimeKind.Unspecified).AddTicks(838), "$2a$11$WBy6GOAupSl1O5usAoXE9uP1AzI1vDHl2D2nuiKEX9hbhaWqlOIGW" });

            migrationBuilder.AddForeignKey(
                name: "FK_comments_issues_IssueId",
                table: "comments",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_projects_projects_ProjectId",
                table: "favorite_projects",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_projects_users_UserId",
                table: "favorite_projects",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_history_issues_IssueId",
                table: "issue_history",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_history_users_ActorId",
                table: "issue_history",
                column: "ActorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_labels_issues_IssueId",
                table: "issue_labels",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_labels_project_labels_LabelId",
                table: "issue_labels",
                column: "LabelId",
                principalTable: "project_labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_statuses_projects_ProjectId",
                table: "issue_statuses",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issue_subtasks_issues_IssueId",
                table: "issue_subtasks",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_issue_statuses_StatusId",
                table: "issues",
                column: "StatusId",
                principalTable: "issue_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_projects_ProjectId",
                table: "issues",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_issues_users_AssigneeId",
                table: "issues",
                column: "AssigneeId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_issues_users_OwnerId",
                table: "issues",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_password_reset_tokens_users_UserId",
                table: "password_reset_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_labels_projects_ProjectId",
                table: "project_labels",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_teams_TeamId",
                table: "projects",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_OwnerId",
                table: "projects",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_activity_logs_teams_TeamId",
                table: "team_activity_logs",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_activity_logs_users_ActorId",
                table: "team_activity_logs",
                column: "ActorId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_teams_TeamId",
                table: "team_invites",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_CreatedBy",
                table: "team_invites",
                column: "CreatedBy",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_teams_TeamId",
                table: "team_members",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_members_users_UserId",
                table: "team_members",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_users_OwnerId",
                table: "teams",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_auth_providers_users_UserId",
                table: "user_auth_providers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
