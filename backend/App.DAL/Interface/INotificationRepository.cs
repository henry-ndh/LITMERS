using App.Entity.DTO.Response.Notification;
using App.Entity.Models;
using App.Entity.Models.Enums;

namespace App.DAL.Interface
{
    public interface INotificationRepository
    {
        // Notification Management
        Task<NotificationModel> CreateNotification(long userId, NotificationType type, string title, string? message = null, string? payload = null);
        Task<NotificationModel> GetNotificationById(long notificationId);
        Task<List<NotificationResponseDTO>> GetNotificationsByUserId(long userId, int limit = 50, bool? isRead = null);
        Task<int> GetUnreadCount(long userId);
        Task<bool> MarkAsRead(long notificationId, long userId);
        Task<bool> MarkAllAsRead(long userId);
        Task<bool> DeleteNotification(long notificationId, long userId);
    }
}

