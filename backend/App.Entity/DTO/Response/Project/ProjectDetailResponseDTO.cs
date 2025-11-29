namespace App.Entity.DTO.Response.Project
{
    public class ProjectDetailResponseDTO
    {
        public long Id { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public bool IsFavorite { get; set; }
        public int IssueCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

