using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("projects")]
    public class ProjectModel : BaseEntity
    {
        [Required]
        [Column("team_id")]
        public long TeamId { get; set; }

        [Required]
        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [StringLength(2000)]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("is_archived")]
        public bool IsArchived { get; set; } = false;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey("TeamId")]
        public virtual TeamModel Team { get; set; }

        [ForeignKey("OwnerId")]
        public virtual UserModel Owner { get; set; }

        public virtual ICollection<IssueStatusModel> IssueStatuses { get; set; }
        public virtual ICollection<IssueModel> Issues { get; set; }
        public virtual ICollection<ProjectLabelModel> Labels { get; set; }
        public virtual ICollection<FavoriteProjectModel> FavoriteProjects { get; set; }
    }
}
