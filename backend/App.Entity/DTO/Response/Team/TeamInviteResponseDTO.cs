namespace App.Entity.DTO.Response.Team
{
    public class TeamInviteResponseDTO
    {
        public long Id { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public long CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExpired { get; set; }
        public bool IsAccepted { get; set; }
    }
}
