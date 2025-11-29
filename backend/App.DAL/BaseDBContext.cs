
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using App.DAL.Implement;
using App.DAL.Interface;
using App.Entity.Models;
using App.Entity.Models.Enums;
using App.Entity.Models.Enums.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.DataBase
{
    public class BaseDBContext : DbContext
    {
        public BaseDBContext(DbContextOptions<BaseDBContext> options) : base(options)
        {
           
        }

        // User & Auth
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserAuthProviderModel> UserAuthProviders { get; set; }
        public DbSet<PasswordResetTokenModel> PasswordResetTokens { get; set; }

        // Team
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamMemberModel> TeamMembers { get; set; }
        public DbSet<TeamInviteModel> TeamInvites { get; set; }
        public DbSet<TeamActivityLogModel> TeamActivityLogs { get; set; }

        // Project
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<FavoriteProjectModel> FavoriteProjects { get; set; }

        // Issue
        public DbSet<IssueStatusModel> IssueStatuses { get; set; }
        public DbSet<IssueModel> Issues { get; set; }
        public DbSet<ProjectLabelModel> ProjectLabels { get; set; }
        public DbSet<IssueLabelModel> IssueLabels { get; set; }
        public DbSet<IssueSubtaskModel> IssueSubtasks { get; set; }
        public DbSet<IssueHistoryModel> IssueHistory { get; set; }

        // Comment
        public DbSet<CommentModel> Comments { get; set; }

        // AI
        public DbSet<AIDailyUsageModel> AIDailyUsage { get; set; }
        public DbSet<AIMinuteUsageModel> AIMinuteUsage { get; set; }
        public DbSet<IssueEmbeddingModel> IssueEmbeddings { get; set; }

        // Notification
        public DbSet<NotificationModel> Notifications { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();

            List<UserModel> users = [
                new UserModel()
                {
                    Id = 1,
                    Name = "Super Admin",
                    Email = "admin@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Role = UserRole.ADMIN,
                },
                new UserModel()
                {
                    Id = 3,
                    Name = "Normal User",
                    Email = "user@gmail.com",
                    Password = passwordHasher.HashPassword("123456"),
                    Role = UserRole.USER
                }
            ];


            builder.Entity<UserModel>()
            .HasData(users);

            // Team configurations
            builder.Entity<TeamModel>()
                .HasIndex(t => t.OwnerId);

            builder.Entity<TeamMemberModel>()
                .HasIndex(tm => new { tm.TeamId, tm.UserId })
                .IsUnique();

            builder.Entity<TeamMemberModel>()
                .HasIndex(tm => tm.TeamId);

            builder.Entity<TeamMemberModel>()
                .HasIndex(tm => tm.UserId);

            builder.Entity<TeamInviteModel>()
                .HasIndex(ti => new { ti.TeamId, ti.Email });

            builder.Entity<TeamInviteModel>()
                .HasIndex(ti => ti.Token)
                .IsUnique();

            builder.Entity<TeamActivityLogModel>()
                .HasIndex(tal => new { tal.TeamId, tal.CreatedAt });

            // User Auth Provider configurations
            builder.Entity<UserAuthProviderModel>()
                .HasIndex(uap => new { uap.Provider, uap.ProviderUserId })
                .IsUnique();

            // Password Reset Token configurations
            builder.Entity<PasswordResetTokenModel>()
                .HasIndex(prt => prt.Token)
                .IsUnique();

            // Project configurations
            builder.Entity<ProjectModel>()
                .HasIndex(p => p.TeamId);

            builder.Entity<ProjectModel>()
                .HasIndex(p => p.OwnerId);

            // Favorite Project configurations (Composite Key)
            builder.Entity<FavoriteProjectModel>()
                .HasKey(fp => new { fp.UserId, fp.ProjectId });

            // Issue Status configurations
            builder.Entity<IssueStatusModel>()
                .HasIndex(ist => new { ist.ProjectId, ist.Name })
                .IsUnique();

            builder.Entity<IssueStatusModel>()
                .HasIndex(ist => new { ist.ProjectId, ist.Position });

            // Issue configurations
            builder.Entity<IssueModel>()
                .HasIndex(i => new { i.ProjectId, i.StatusId, i.Position });

            builder.Entity<IssueModel>()
                .HasIndex(i => i.AssigneeId);

            builder.Entity<IssueModel>()
                .HasIndex(i => i.Priority);

            builder.Entity<IssueModel>()
                .HasIndex(i => i.DueDate);

            // Project Label configurations
            builder.Entity<ProjectLabelModel>()
                .HasIndex(pl => new { pl.ProjectId, pl.Name })
                .IsUnique();

            builder.Entity<ProjectLabelModel>()
                .HasIndex(pl => pl.ProjectId);

            // Issue Label configurations (Composite Key)
            builder.Entity<IssueLabelModel>()
                .HasKey(il => new { il.IssueId, il.LabelId });

            // Issue Subtask configurations
            builder.Entity<IssueSubtaskModel>()
                .HasIndex(ist => new { ist.IssueId, ist.Position });

            // Issue History configurations
            builder.Entity<IssueHistoryModel>()
                .HasIndex(ih => new { ih.IssueId, ih.CreatedAt });

            // Comment configurations
            builder.Entity<CommentModel>()
                .HasIndex(c => new { c.IssueId, c.CreatedAt });

            builder.Entity<CommentModel>()
                .HasIndex(c => c.UserId);

            // AI Daily Usage configurations
            builder.Entity<AIDailyUsageModel>()
                .HasIndex(adu => new { adu.UserId, adu.Date })
                .IsUnique();

            builder.Entity<AIDailyUsageModel>()
                .HasIndex(adu => adu.UserId);

            // AI Minute Usage configurations
            builder.Entity<AIMinuteUsageModel>()
                .HasIndex(amu => new { amu.UserId, amu.MinuteBucket })
                .IsUnique();

            builder.Entity<AIMinuteUsageModel>()
                .HasIndex(amu => amu.UserId);

            // Issue Embedding configurations
            builder.Entity<IssueEmbeddingModel>()
                .HasIndex(ie => ie.IssueId)
                .IsUnique();

            // Notification configurations
            builder.Entity<NotificationModel>()
                .HasIndex(n => new { n.UserId, n.CreatedAt });

            builder.Entity<NotificationModel>()
                .HasIndex(n => new { n.UserId, n.IsRead });

        }

        

       

    }
}
