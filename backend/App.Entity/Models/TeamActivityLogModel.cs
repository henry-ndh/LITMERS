using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;
using App.Entity.Models.Enums;

namespace App.Entity.Models
{
    [Table("team_activity_logs")]
    public class TeamActivityLogModel : BaseEntity
    {
        [Required]
        [Column("team_id")]
        public long TeamId { get; set; }

        [Required]
        [Column("actor_id")]
        public long ActorId { get; set; }

        [Required]
        [Column("action_type")]
        public ActivityActionType ActionType { get; set; }

        [Column("target_id")]
        public long? TargetId { get; set; }

        [StringLength(50)]
        [Column("target_type")]
        public string? TargetType { get; set; }

        [Column("message")]
        public string? Message { get; set; }

        [Column("metadata")]
        public string? Metadata { get; set; }

        // Navigation properties
        [ForeignKey("TeamId")]
        public virtual TeamModel Team { get; set; }

        [ForeignKey("ActorId")]
        public virtual UserModel Actor { get; set; }
    }
}
