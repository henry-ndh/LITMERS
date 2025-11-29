using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("issue_statuses")]
    public class IssueStatusModel : BaseEntity
    {
        [Required]
        [Column("project_id")]
        public long ProjectId { get; set; }

        [Required]
        [StringLength(30)]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(7)]
        [Column("color")]
        public string? Color { get; set; }

        [Required]
        [Column("position")]
        public int Position { get; set; }

        [Required]
        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("wip_limit")]
        public int? WipLimit { get; set; } // null = unlimited

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual ProjectModel Project { get; set; }

        public virtual ICollection<IssueModel> Issues { get; set; }
    }
}
