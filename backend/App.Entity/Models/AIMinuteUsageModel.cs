using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("ai_minute_usage")]
    public class AIMinuteUsageModel : BaseEntity
    {
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("minute_bucket")]
        public DateTime MinuteBucket { get; set; } // rounded to minute

        [Required]
        [Column("count")]
        public int Count { get; set; } = 0;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
    }
}
