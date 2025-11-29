using App.Entity.DTO.Response.Notification;

namespace App.BLL.Interface
{
    public interface INotificationBiz
    {
        // Notification Management
        Task<NotificationResponseDTO> GetNotificationById(long notificationId, long userId);
        Task<NotificationSummaryResponseDTO> GetNotifications(long userId, int limit = 50, bool? isRead = null);
        Task<int> GetUnreadCount(long userId);
        Task<bool> MarkAsRead(long notificationId, long userId);
        Task<bool> MarkAllAsRead(long userId);
        Task<bool> DeleteNotification(long notificationId, long userId);
    }
}

