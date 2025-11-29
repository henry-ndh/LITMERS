namespace App.Entity.DTO.Response.Notification
{
    public class NotificationSummaryResponseDTO
    {
        public int UnreadCount { get; set; }
        public List<NotificationResponseDTO> Notifications { get; set; }
    }
}

