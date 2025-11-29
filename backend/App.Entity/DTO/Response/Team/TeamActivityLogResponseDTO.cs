using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response.Team
{
    public class TeamActivityLogResponseDTO
    {
        public long Id { get; set; }
        public long TeamId { get; set; }
        public long ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorAvatar { get; set; }
        public ActivityActionType ActionType { get; set; }
        public string ActionTypeName { get; set; }
        public long? TargetId { get; set; }
        public string TargetType { get; set; }
        public string Message { get; set; }
        public string Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
