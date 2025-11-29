using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Response.Notification;
using App.Entity.Models;
using App.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class NotificationRepository : AppBaseRepository, INotificationRepository
    {
        private readonly BaseDBContext _dbContext;

        public NotificationRepository(BaseDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region Notification Management

        public async Task<NotificationModel> CreateNotification(long userId, NotificationType type, string title, string? message = null, string? payload = null)
        {
            var notification = new NotificationModel
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                Payload = payload,
                IsRead = false,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<NotificationModel>().AddAsync(notification);
            await _dbContext.SaveChangesAsync();

            return notification;
        }

        public async Task<NotificationModel> GetNotificationById(long notificationId)
        {
            return await _dbContext.Set<NotificationModel>()
                .Where(n => n.Id == notificationId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<NotificationResponseDTO>> GetNotificationsByUserId(long userId, int limit = 50, bool? isRead = null)
        {
            var query = _dbContext.Set<NotificationModel>()
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .Select(n => new NotificationResponseDTO
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Type = n.Type,
                    TypeName = n.Type.ToString(),
                    Title = n.Title,
                    Message = n.Message,
                    Payload = n.Payload,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount(long userId)
        {
            return await _dbContext.Set<NotificationModel>()
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> MarkAsRead(long notificationId, long userId)
        {
            var notification = await _dbContext.Set<NotificationModel>()
                .Where(n => n.Id == notificationId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            notification.IsRead = true;
            _dbContext.Set<NotificationModel>().Update(notification);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAllAsRead(long userId)
        {
            var unreadNotifications = await _dbContext.Set<NotificationModel>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            if (unreadNotifications.Any())
            {
                _dbContext.Set<NotificationModel>().UpdateRange(unreadNotifications);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteNotification(long notificationId, long userId)
        {
            var notification = await _dbContext.Set<NotificationModel>()
                .Where(n => n.Id == notificationId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            _dbContext.Set<NotificationModel>().Remove(notification);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}

