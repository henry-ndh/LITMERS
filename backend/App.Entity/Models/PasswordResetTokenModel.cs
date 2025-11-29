using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("password_reset_tokens")]
    public class PasswordResetTokenModel : BaseEntity
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("token")]
        public string Token { get; set; }

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("used_at")]
        public DateTime? UsedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
