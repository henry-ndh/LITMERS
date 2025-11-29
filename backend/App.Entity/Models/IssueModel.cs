using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;
using App.Entity.Models.Enums;

namespace App.Entity.Models
{
    [Table("issues")]
    public class IssueModel : BaseEntity
    {
        [Required]
        [Column("project_id")]
        public long ProjectId { get; set; }

        [Required]
        [Column("status_id")]
        public long StatusId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("assignee_id")]
        public long? AssigneeId { get; set; }

        [Column("due_date", TypeName = "date")]
        public DateTime? DueDate { get; set; }

        [Required]
        [Column("priority")]
        public IssuePriority Priority { get; set; } = IssuePriority.MEDIUM;

        [Required]
        [Column("position")]
        public int Position { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // AI-generated fields (cached in issue table)
        [Column("ai_summary", TypeName = "text")]
        public string? AiSummary { get; set; }

        [Column("ai_summary_generated_at")]
        public DateTime? AiSummaryGeneratedAt { get; set; }

        [Column("ai_suggestion", TypeName = "text")]
        public string? AiSuggestion { get; set; }

        [Column("ai_suggestion_generated_at")]
        public DateTime? AiSuggestionGeneratedAt { get; set; }

        [Column("ai_comment_summary", TypeName = "text")]
        public string? AiCommentSummary { get; set; } // JSON string: {"summary": "...", "decisions": [...]}

        [Column("ai_comment_summary_generated_at")]
        public DateTime? AiCommentSummaryGeneratedAt { get; set; }

        [Column("ai_comment_summary_comment_count")]
        public int? AiCommentSummaryCommentCount { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual ProjectModel Project { get; set; }

        [ForeignKey("StatusId")]
        public virtual IssueStatusModel Status { get; set; }

        [ForeignKey("OwnerId")]
        public virtual UserModel Owner { get; set; }

        [ForeignKey("AssigneeId")]
        public virtual UserModel Assignee { get; set; }

        public virtual ICollection<IssueLabelModel> IssueLabels { get; set; }
        public virtual ICollection<IssueSubtaskModel> Subtasks { get; set; }
        public virtual ICollection<IssueHistoryModel> History { get; set; }
        public virtual ICollection<CommentModel> Comments { get; set; }
        public virtual IssueEmbeddingModel Embedding { get; set; }
    }
}
