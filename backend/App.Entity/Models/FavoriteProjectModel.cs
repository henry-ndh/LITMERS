using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Entity.Models
{
    [Table("favorite_projects")]
    public class FavoriteProjectModel
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("project_id")]
        public long ProjectId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [ForeignKey("ProjectId")]
        public virtual ProjectModel Project { get; set; }
    }
}
