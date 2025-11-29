using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("project_labels")]
    public class ProjectLabelModel : BaseEntity
    {
        [Required]
        [Column("project_id")]
        public long ProjectId { get; set; }

        [Required]
        [StringLength(30)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [StringLength(7)]
        [Column("color")]
        public string Color { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual ProjectModel Project { get; set; }

        public virtual ICollection<IssueLabelModel> IssueLabels { get; set; }
    }
}
