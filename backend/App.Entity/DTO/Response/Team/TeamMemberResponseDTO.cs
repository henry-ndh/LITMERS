using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response.Team
{
    public class TeamMemberResponseDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public TeamRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
