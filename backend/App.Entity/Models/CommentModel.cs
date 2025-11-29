using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("comments")]
    public class CommentModel : BaseEntity
    {
        [Required]
        [Column("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [StringLength(1000)]
        [Column("content")]
        public string Content { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual IssueModel Issue { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
