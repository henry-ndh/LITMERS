using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models
{
    [Table("issue_labels")]
    public class IssueLabelModel
    {
        [Required]
        [Column("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [Column("label_id")]
        public long LabelId { get; set; }

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual IssueModel Issue { get; set; }

        [ForeignKey("LabelId")]
        public virtual ProjectLabelModel Label { get; set; }
    }
}
