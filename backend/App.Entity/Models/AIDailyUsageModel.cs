using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("ai_daily_usage")]
    public class AIDailyUsageModel : BaseEntity
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [Column("count")]
        public int Count { get; set; } = 0;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
