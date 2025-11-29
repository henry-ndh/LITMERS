using App.Entity.DTO.Request.Comment;
using App.Entity.DTO.Response.Comment;
using App.Entity.Models;

namespace App.DAL.Interface
{
    public interface ICommentRepository
    {
        // Comment Management
        Task<CommentModel> CreateComment(long userId, CreateCommentRequestDTO dto);
        Task<CommentModel> UpdateComment(long commentId, long userId, UpdateCommentRequestDTO dto);
        Task<bool> DeleteComment(long commentId, long userId);
        Task<CommentModel> GetCommentById(long commentId);
        Task<List<CommentResponseDTO>> GetCommentsByIssueId(long issueId, long currentUserId);
        Task<bool> IsCommentOwner(long commentId, long userId);
        Task<bool> HasCommentAccess(long commentId, long userId);
    }
}

