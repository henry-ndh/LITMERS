using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("issue_history")]
    public class IssueHistoryModel : BaseEntity
    {
        [Required]
        [Column("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("field")]
        public string Field { get; set; } // status, assignee, priority, title, due_date

        [Column("old_value", TypeName = "text")]
        public string? OldValue { get; set; }

        [Column("new_value", TypeName = "text")]
        public string? NewValue { get; set; }

        [Required]
        [Column("actor_id")]
        public long ActorId { get; set; }

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual IssueModel Issue { get; set; }

        [ForeignKey("ActorId")]
        public virtual UserModel Actor { get; set; }
    }
}
