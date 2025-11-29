using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;
using App.Entity.Models.Enums;

namespace App.Entity.Models
{
    [Table("notifications")]
    public class NotificationModel : BaseEntity
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("type")]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; }

        [Column("message", TypeName = "text")]
        public string? Message { get; set; }

        [Column("payload", TypeName = "text")]
        public string? Payload { get; set; } // JSON string

        [Required]
        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
