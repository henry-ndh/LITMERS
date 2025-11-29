using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("user_auth_providers")]
    public class UserAuthProviderModel : BaseEntity
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("provider")]
        public string Provider { get; set; } // 'google'

        [Required]
        [StringLength(255)]
        [Column("provider_user_id")]
        public string ProviderUserId { get; set; }

        [StringLength(255)]
        [Column("email")]
        public string? Email { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
