namespace App.Entity.DTO.Response.Issue
{
    public class IssueHistoryResponseDTO
    {
        public long Id { get; set; }
        public long IssueId { get; set; }
        public string Field { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

