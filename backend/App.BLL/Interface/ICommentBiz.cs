using App.Entity.DTO.Request.Comment;
using App.Entity.DTO.Response.Comment;

namespace App.BLL.Interface
{
    public interface ICommentBiz
    {
        // Comment Management
        Task<CommentResponseDTO> CreateComment(long userId, CreateCommentRequestDTO dto);
        Task<CommentResponseDTO> UpdateComment(long commentId, long userId, UpdateCommentRequestDTO dto);
        Task<bool> DeleteComment(long commentId, long userId);
        Task<CommentResponseDTO> GetCommentById(long commentId, long userId);
        Task<List<CommentResponseDTO>> GetCommentsByIssueId(long issueId, long userId);
    }
}

