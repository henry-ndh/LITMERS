using App.BLL.Interface;
using App.Entity.DTO.Request.Comment;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : BaseAPIController
    {
        private readonly ICommentBiz _commentBiz;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentBiz commentBiz, ILogger<CommentController> logger)
        {
            _commentBiz = commentBiz;
            _logger = logger;
        }

        #region Comment Management

        /// <summary>
        /// Create a new comment on an issue
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _commentBiz.CreateComment(UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateComment] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(long commentId, [FromBody] UpdateCommentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _commentBiz.UpdateComment(commentId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateComment] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a comment (soft delete)
        /// </summary>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(long commentId)
        {
            try
            {
                var result = await _commentBiz.DeleteComment(commentId, UserId);
                return Success("Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteComment] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get comment by ID
        /// </summary>
        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetCommentById(long commentId)
        {
            try
            {
                var result = await _commentBiz.GetCommentById(commentId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetCommentById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all comments for an issue
        /// </summary>
        [HttpGet("issue/{issueId}")]
        public async Task<IActionResult> GetCommentsByIssueId(long issueId)
        {
            try
            {
                var result = await _commentBiz.GetCommentsByIssueId(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetCommentsByIssueId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion
    }
}

