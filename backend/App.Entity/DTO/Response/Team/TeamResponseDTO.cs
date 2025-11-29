using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response.Team
{
    public class TeamResponseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public int MemberCount { get; set; }
        public TeamRole? CurrentUserRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
