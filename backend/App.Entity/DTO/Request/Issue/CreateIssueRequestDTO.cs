using System.ComponentModel.DataAnnotations;
using App.Entity.Models.Enums;

namespace App.Entity.DTO.Request.Issue
{
    public class CreateIssueRequestDTO
    {
        [Required(ErrorMessage = "Project ID is required")]
        public long ProjectId { get; set; }

        [Required(ErrorMessage = "Status ID is required")]
        public long StatusId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public long? AssigneeId { get; set; }

        public DateTime? DueDate { get; set; }

        public IssuePriority Priority { get; set; } = IssuePriority.MEDIUM;

        [Required(ErrorMessage = "Position is required")]
        public int Position { get; set; }

        public List<long>? LabelIds { get; set; }
    }
}

