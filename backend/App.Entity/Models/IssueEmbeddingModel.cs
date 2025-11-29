using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entity.Common;

namespace App.Entity.Models
{
    [Table("issue_embeddings")]
    public class IssueEmbeddingModel : BaseEntity
    {
        [Required]
        [Column("issue_id")]
        public long IssueId { get; set; }

        [Required]
        [Column("embedding", TypeName = "text")]
        public string Embedding { get; set; } // vector stored as JSON string

        // Note: UpdatedAt is already in BaseEntity

        // Navigation properties
        [ForeignKey("IssueId")]
        public virtual IssueModel Issue { get; set; }
    }
}
