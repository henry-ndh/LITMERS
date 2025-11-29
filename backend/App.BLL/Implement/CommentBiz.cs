using App.BLL.Interface;
using App.DAL.Interface;
using App.Entity.DTO.Request.Comment;
using App.Entity.DTO.Response.Comment;
using App.Entity.Models;
using AutoMapper;

namespace App.BLL.Implement
{
    public class CommentBiz : ICommentBiz
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentBiz(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        #region Comment Management

        public async Task<CommentResponseDTO> CreateComment(long userId, CreateCommentRequestDTO dto)
        {
            var comment = await _commentRepository.CreateComment(userId, dto);
            return await MapToCommentResponse(comment, userId);
        }

        public async Task<CommentResponseDTO> UpdateComment(long commentId, long userId, UpdateCommentRequestDTO dto)
        {
            var comment = await _commentRepository.UpdateComment(commentId, userId, dto);
            return await MapToCommentResponse(comment, userId);
        }

        public async Task<bool> DeleteComment(long commentId, long userId)
        {
            return await _commentRepository.DeleteComment(commentId, userId);
        }

        public async Task<CommentResponseDTO> GetCommentById(long commentId, long userId)
        {
            var comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            // Check access
            if (!await _commentRepository.HasCommentAccess(commentId, userId))
            {
                throw new Exception("You don't have permission to view this comment");
            }

            return await MapToCommentResponse(comment, userId);
        }

        public async Task<List<CommentResponseDTO>> GetCommentsByIssueId(long issueId, long userId)
        {
            return await _commentRepository.GetCommentsByIssueId(issueId, userId);
        }

        #endregion

        #region Helper Methods

        private async Task<CommentResponseDTO> MapToCommentResponse(CommentModel comment, long currentUserId)
        {
            return new CommentResponseDTO
            {
                Id = comment.Id,
                IssueId = comment.IssueId,
                IssueTitle = comment.Issue?.Title,
                UserId = comment.UserId,
                UserName = comment.User?.Name,
                UserEmail = comment.User?.Email,
                UserAvatar = comment.User?.Avatar,
                Content = comment.Content,
                IsOwner = comment.UserId == currentUserId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        #endregion
    }
}

