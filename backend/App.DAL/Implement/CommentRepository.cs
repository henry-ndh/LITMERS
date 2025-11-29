using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request.Comment;
using App.Entity.DTO.Response.Comment;
using App.Entity.Models;
using Base.Common;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class CommentRepository : AppBaseRepository, ICommentRepository
    {
        private readonly BaseDBContext _dbContext;
        private readonly IIssueRepository _issueRepository;
        private readonly INotificationRepository _notificationRepository;

        public CommentRepository(BaseDBContext dbContext, IIssueRepository issueRepository, INotificationRepository notificationRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _issueRepository = issueRepository;
            _notificationRepository = notificationRepository;
        }

        #region Comment Management

        public async Task<CommentModel> CreateComment(long userId, CreateCommentRequestDTO dto)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(dto.IssueId, userId))
            {
                throw new Exception("You don't have permission to comment on this issue");
            }

            var comment = new CommentModel
            {
                IssueId = dto.IssueId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<CommentModel>().AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            // Notify issue owner and assignee
            var issue = await _issueRepository.GetIssueById(dto.IssueId);
            if (issue != null)
            {
                var commenter = await _dbContext.Users.FindAsync(userId);
                var payload = System.Text.Json.JsonSerializer.Serialize(new { issueId = issue.Id, issueTitle = issue.Title, commentId = comment.Id });

                // Notify issue owner (if not the commenter)
                if (issue.OwnerId != userId)
                {
                    await _notificationRepository.CreateNotification(
                        issue.OwnerId,
                        App.Entity.Models.Enums.NotificationType.ISSUE_COMMENTED,
                        $"New comment on issue: {issue.Title}",
                        $"{commenter?.Name} commented on issue '{issue.Title}'",
                        payload
                    );
                }

                // Notify assignee (if exists and not the commenter and not the owner)
                if (issue.AssigneeId.HasValue && issue.AssigneeId.Value != userId && issue.AssigneeId.Value != issue.OwnerId)
                {
                    await _notificationRepository.CreateNotification(
                        issue.AssigneeId.Value,
                        App.Entity.Models.Enums.NotificationType.ISSUE_COMMENTED,
                        $"New comment on issue: {issue.Title}",
                        $"{commenter?.Name} commented on issue '{issue.Title}'",
                        payload
                    );
                }
            }

            return comment;
        }

        public async Task<CommentModel> UpdateComment(long commentId, long userId, UpdateCommentRequestDTO dto)
        {
            var comment = await GetCommentById(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            // Check if user is the owner
            if (comment.UserId != userId)
            {
                throw new Exception("Only comment owner can update the comment");
            }

            comment.Content = dto.Content;
            comment.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<CommentModel>().Update(comment);
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<bool> DeleteComment(long commentId, long userId)
        {
            var comment = await GetCommentById(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            // Check if user is the owner
            if (comment.UserId != userId)
            {
                throw new Exception("Only comment owner can delete the comment");
            }

            comment.DeletedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<CommentModel>().Update(comment);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<CommentModel> GetCommentById(long commentId)
        {
            return await _dbContext.Set<CommentModel>()
                .Include(c => c.Issue)
                .Include(c => c.User)
                .Where(c => c.Id == commentId && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CommentResponseDTO>> GetCommentsByIssueId(long issueId, long currentUserId)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, currentUserId))
            {
                throw new Exception("You don't have permission to view comments for this issue");
            }

            return await _dbContext.Set<CommentModel>()
                .Include(c => c.Issue)
                .Include(c => c.User)
                .Where(c => c.IssueId == issueId && c.DeletedAt == null)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponseDTO
                {
                    Id = c.Id,
                    IssueId = c.IssueId,
                    IssueTitle = c.Issue.Title,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserEmail = c.User.Email,
                    UserAvatar = c.User.Avatar,
                    Content = c.Content,
                    IsOwner = c.UserId == currentUserId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> IsCommentOwner(long commentId, long userId)
        {
            var comment = await GetCommentById(commentId);
            if (comment == null) return false;

            return comment.UserId == userId;
        }

        public async Task<bool> HasCommentAccess(long commentId, long userId)
        {
            var comment = await GetCommentById(commentId);
            if (comment == null) return false;

            return await _issueRepository.HasIssueAccess(comment.IssueId, userId);
        }

        #endregion
    }
}

