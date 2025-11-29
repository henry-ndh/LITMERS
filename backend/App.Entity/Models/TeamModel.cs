using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("teams")]
    public class TeamModel : BaseEntity
    {
        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey("OwnerId")]
        public virtual UserModel Owner { get; set; }

        public virtual ICollection<TeamMemberModel> TeamMembers { get; set; }
        public virtual ICollection<TeamInviteModel> TeamInvites { get; set; }
        public virtual ICollection<TeamActivityLogModel> ActivityLogs { get; set; }
    }
}
