namespace App.Entity.DTO.Response.Issue
{
    public class IssueStatusResponseDTO
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public int Position { get; set; }
        public bool IsDefault { get; set; }
        public int? WipLimit { get; set; }
        public int IssueCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

