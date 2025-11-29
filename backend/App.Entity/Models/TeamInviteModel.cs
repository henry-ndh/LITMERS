using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("team_invites")]
    public class TeamInviteModel : BaseEntity
    {
        [Required]
        [Column("team_id")]
        public long TeamId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Column("token")]
        public string Token { get; set; }

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("accepted_at")]
        public DateTime? AcceptedAt { get; set; }

        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("TeamId")]
        public virtual TeamModel Team { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserModel Creator { get; set; }
    }
}
