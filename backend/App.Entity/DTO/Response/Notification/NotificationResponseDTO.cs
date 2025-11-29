using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response.Notification
{
    public class NotificationResponseDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public NotificationType Type { get; set; }
        public string TypeName { get; set; }
        public string Title { get; set; }
        public string? Message { get; set; }
        public string? Payload { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

