using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Comment
{
    public class CreateCommentRequestDTO
    {
        [Required(ErrorMessage = "Issue ID is required")]
        public long IssueId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; }
    }
}

