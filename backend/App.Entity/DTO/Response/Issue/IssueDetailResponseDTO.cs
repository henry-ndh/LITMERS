using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response.Issue
{
    public class IssueDetailResponseDTO
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public long? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public string? AssigneeEmail { get; set; }
        public DateTime? DueDate { get; set; }
        public IssuePriority Priority { get; set; }
        public int Position { get; set; }
        public List<LabelResponseDTO> Labels { get; set; }
        public List<SubtaskResponseDTO> Subtasks { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

