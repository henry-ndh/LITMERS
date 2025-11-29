namespace App.Entity.DTO.Response.Issue
{
    public class SubtaskResponseDTO
    {
        public long Id { get; set; }
        public long IssueId { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public int Position { get; set; }
        public long? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public string? AssigneeEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

