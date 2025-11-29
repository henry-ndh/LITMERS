using App.BLL.Interface;
using App.DAL.Interface;
using App.Entity.DTO.Response.Notification;
using App.Entity.Models;
using AutoMapper;

namespace App.BLL.Implement
{
    public class NotificationBiz : INotificationBiz
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationBiz(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        #region Notification Management

        public async Task<NotificationResponseDTO> GetNotificationById(long notificationId, long userId)
        {
            var notification = await _notificationRepository.GetNotificationById(notificationId);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            // Check if notification belongs to user
            if (notification.UserId != userId)
            {
                throw new Exception("You don't have permission to view this notification");
            }

            return _mapper.Map<NotificationResponseDTO>(notification);
        }

        public async Task<NotificationSummaryResponseDTO> GetNotifications(long userId, int limit = 50, bool? isRead = null)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserId(userId, limit, isRead);
            var unreadCount = await _notificationRepository.GetUnreadCount(userId);

            return new NotificationSummaryResponseDTO
            {
                UnreadCount = unreadCount,
                Notifications = notifications
            };
        }

        public async Task<int> GetUnreadCount(long userId)
        {
            return await _notificationRepository.GetUnreadCount(userId);
        }

        public async Task<bool> MarkAsRead(long notificationId, long userId)
        {
            return await _notificationRepository.MarkAsRead(notificationId, userId);
        }

        public async Task<bool> MarkAllAsRead(long userId)
        {
            return await _notificationRepository.MarkAllAsRead(userId);
        }

        public async Task<bool> DeleteNotification(long notificationId, long userId)
        {
            return await _notificationRepository.DeleteNotification(notificationId, userId);
        }

        #endregion
    }
}

