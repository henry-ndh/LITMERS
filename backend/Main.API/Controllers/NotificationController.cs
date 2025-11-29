using App.BLL.Interface;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : BaseAPIController
    {
        private readonly INotificationBiz _notificationBiz;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationBiz notificationBiz, ILogger<NotificationController> logger)
        {
            _notificationBiz = notificationBiz;
            _logger = logger;
        }

        #region Notification Management

        /// <summary>
        /// Get notification by ID
        /// </summary>
        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetNotificationById(long notificationId)
        {
            try
            {
                var result = await _notificationBiz.GetNotificationById(notificationId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetNotificationById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all notifications for current user with unread count
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int limit = 50, [FromQuery] bool? isRead = null)
        {
            try
            {
                var result = await _notificationBiz.GetNotifications(UserId, limit, isRead);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetNotifications] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var result = await _notificationBiz.GetUnreadCount(UserId);
                return GetSuccess(new { unreadCount = result });
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetUnreadCount] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(long notificationId)
        {
            try
            {
                var result = await _notificationBiz.MarkAsRead(notificationId, UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[MarkAsRead] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var result = await _notificationBiz.MarkAllAsRead(UserId);
                return Success("All notifications marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError("[MarkAllAsRead] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(long notificationId)
        {
            try
            {
                var result = await _notificationBiz.DeleteNotification(notificationId, UserId);
                return Success("Notification deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteNotification] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion
    }
}

