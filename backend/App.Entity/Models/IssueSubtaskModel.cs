using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("issue_subtasks")]
    public class IssueSubtaskModel : BaseEntity
    {
        [Required]
        [Column("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("is_done")]
        public bool IsDone { get; set; } = false;

        [Required]
        [Column("position")]
        public int Position { get; set; }

        [Column("assignee_id")]
        public long? AssigneeId { get; set; }

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual IssueModel Issue { get; set; }

        [ForeignKey("AssigneeId")]
        public virtual UserModel Assignee { get; set; }
    }
}
