using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;
using App.Entity.Models.Enums;

namespace App.Entity.Models
{
    [Table("team_members")]
    public class TeamMemberModel : BaseEntity
    {
        [Required]
        [Column("team_id")]
        public long TeamId { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("role")]
        public TeamRole Role { get; set; }

        // Navigation properties
        [ForeignKey("TeamId")]
        public virtual TeamModel Team { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
