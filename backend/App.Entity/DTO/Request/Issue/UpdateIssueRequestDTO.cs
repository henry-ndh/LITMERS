using System.ComponentModel.DataAnnotations;
using App.Entity.Models.Enums;

namespace App.Entity.DTO.Request.Issue
{
    public class UpdateIssueRequestDTO
    {
        public long? StatusId { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public long? AssigneeId { get; set; }

        public DateTime? DueDate { get; set; }

        public IssuePriority? Priority { get; set; }

        public int? Position { get; set; }

        public List<long>? LabelIds { get; set; }
    }
}

