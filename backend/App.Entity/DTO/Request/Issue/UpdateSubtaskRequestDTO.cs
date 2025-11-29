using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Issue
{
    public class UpdateSubtaskRequestDTO
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        public bool? IsDone { get; set; }

        public int? Position { get; set; }

        public long? AssigneeId { get; set; }
    }
}

