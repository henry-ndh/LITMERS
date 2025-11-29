using App.BLL.Interface;
using App.Entity.DTO.Request.Issue;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IssueController : BaseAPIController
    {
        private readonly IIssueBiz _issueBiz;
        private readonly IAIService _aiService;
        private readonly ILogger<IssueController> _logger;

        public IssueController(IIssueBiz issueBiz, IAIService aiService, ILogger<IssueController> logger)
        {
            _issueBiz = issueBiz;
            _aiService = aiService;
            _logger = logger;
        }

        #region Issue Status Management

        /// <summary>
        /// Create a new issue status (column) for a project
        /// </summary>
        [HttpPost("status")]
        public async Task<IActionResult> CreateIssueStatus([FromQuery] long projectId, [FromBody] CreateIssueStatusRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.CreateIssueStatus(projectId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateIssueStatus] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update an issue status
        /// </summary>
        [HttpPut("status/{statusId}")]
        public async Task<IActionResult> UpdateIssueStatus(long statusId, [FromQuery] long projectId, [FromBody] UpdateIssueStatusRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.UpdateIssueStatus(statusId, projectId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateIssueStatus] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete an issue status
        /// </summary>
        [HttpDelete("status/{statusId}")]
        public async Task<IActionResult> DeleteIssueStatus(long statusId, [FromQuery] long projectId)
        {
            try
            {
                var result = await _issueBiz.DeleteIssueStatus(statusId, projectId, UserId);
                return Success("Status deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteIssueStatus] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get issue status by ID
        /// </summary>
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetIssueStatusById(long statusId)
        {
            try
            {
                var result = await _issueBiz.GetIssueStatusById(statusId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueStatusById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all issue statuses for a project
        /// </summary>
        [HttpGet("status/project/{projectId}")]
        public async Task<IActionResult> GetIssueStatusesByProjectId(long projectId)
        {
            try
            {
                var result = await _issueBiz.GetIssueStatusesByProjectId(projectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueStatusesByProjectId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Reorder issue statuses
        /// </summary>
        [HttpPut("status/reorder")]
        public async Task<IActionResult> ReorderStatuses([FromQuery] long projectId, [FromBody] List<long> statusIds)
        {
            try
            {
                var result = await _issueBiz.ReorderStatuses(projectId, UserId, statusIds);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ReorderStatuses] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Issue Management

        /// <summary>
        /// Create a new issue
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] CreateIssueRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.CreateIssue(UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update an issue
        /// </summary>
        [HttpPut("{issueId}")]
        public async Task<IActionResult> UpdateIssue(long issueId, [FromBody] UpdateIssueRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.UpdateIssue(issueId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete an issue (soft delete)
        /// </summary>
        [HttpDelete("{issueId}")]
        public async Task<IActionResult> DeleteIssue(long issueId)
        {
            try
            {
                var result = await _issueBiz.DeleteIssue(issueId, UserId);
                return Success("Issue deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get issue by ID
        /// </summary>
        [HttpGet("{issueId}")]
        public async Task<IActionResult> GetIssueById(long issueId)
        {
            try
            {
                var result = await _issueBiz.GetIssueById(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get issue detail with subtasks and labels
        /// </summary>
        [HttpGet("{issueId}/detail")]
        public async Task<IActionResult> GetIssueDetail(long issueId)
        {
            try
            {
                var result = await _issueBiz.GetIssueDetail(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueDetail] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all issues in a project
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetIssuesByProjectId(long projectId)
        {
            try
            {
                var result = await _issueBiz.GetIssuesByProjectId(projectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssuesByProjectId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all issues in a status (column)
        /// </summary>
        [HttpGet("status/{statusId}/issues")]
        public async Task<IActionResult> GetIssuesByStatusId(long statusId)
        {
            try
            {
                var result = await _issueBiz.GetIssuesByStatusId(statusId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssuesByStatusId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Move an issue to a different status and position
        /// </summary>
        [HttpPut("{issueId}/move")]
        public async Task<IActionResult> MoveIssue(long issueId, [FromBody] MoveIssueRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.MoveIssue(issueId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[MoveIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Project Label Management

        /// <summary>
        /// Create a new label for a project
        /// </summary>
        [HttpPost("label")]
        public async Task<IActionResult> CreateProjectLabel([FromQuery] long projectId, [FromBody] CreateProjectLabelRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.CreateProjectLabel(projectId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateProjectLabel] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update a project label
        /// </summary>
        [HttpPut("label/{labelId}")]
        public async Task<IActionResult> UpdateProjectLabel(long labelId, [FromQuery] long projectId, [FromBody] UpdateProjectLabelRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.UpdateProjectLabel(labelId, projectId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateProjectLabel] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a project label
        /// </summary>
        [HttpDelete("label/{labelId}")]
        public async Task<IActionResult> DeleteProjectLabel(long labelId, [FromQuery] long projectId)
        {
            try
            {
                var result = await _issueBiz.DeleteProjectLabel(labelId, projectId, UserId);
                return Success("Label deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteProjectLabel] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get label by ID
        /// </summary>
        [HttpGet("label/{labelId}")]
        public async Task<IActionResult> GetProjectLabelById(long labelId)
        {
            try
            {
                var result = await _issueBiz.GetProjectLabelById(labelId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetProjectLabelById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all labels for a project
        /// </summary>
        [HttpGet("label/project/{projectId}")]
        public async Task<IActionResult> GetProjectLabels(long projectId)
        {
            try
            {
                var result = await _issueBiz.GetProjectLabels(projectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetProjectLabels] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion

        #region Issue Label Management

        /// <summary>
        /// Add a label to an issue
        /// </summary>
        [HttpPost("{issueId}/label/{labelId}")]
        public async Task<IActionResult> AddLabelToIssue(long issueId, long labelId)
        {
            try
            {
                var result = await _issueBiz.AddLabelToIssue(issueId, labelId, UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[AddLabelToIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Remove a label from an issue
        /// </summary>
        [HttpDelete("{issueId}/label/{labelId}")]
        public async Task<IActionResult> RemoveLabelFromIssue(long issueId, long labelId)
        {
            try
            {
                var result = await _issueBiz.RemoveLabelFromIssue(issueId, labelId, UserId);
                return Success("Label removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[RemoveLabelFromIssue] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update all labels for an issue
        /// </summary>
        [HttpPut("{issueId}/labels")]
        public async Task<IActionResult> UpdateIssueLabels(long issueId, [FromBody] List<long> labelIds)
        {
            try
            {
                var result = await _issueBiz.UpdateIssueLabels(issueId, UserId, labelIds);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateIssueLabels] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Subtask Management

        /// <summary>
        /// Create a new subtask for an issue
        /// </summary>
        [HttpPost("{issueId}/subtask")]
        public async Task<IActionResult> CreateSubtask(long issueId, [FromBody] CreateSubtaskRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.CreateSubtask(issueId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateSubtask] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update a subtask
        /// </summary>
        [HttpPut("subtask/{subtaskId}")]
        public async Task<IActionResult> UpdateSubtask(long subtaskId, [FromQuery] long issueId, [FromBody] UpdateSubtaskRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _issueBiz.UpdateSubtask(subtaskId, issueId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateSubtask] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a subtask
        /// </summary>
        [HttpDelete("subtask/{subtaskId}")]
        public async Task<IActionResult> DeleteSubtask(long subtaskId, [FromQuery] long issueId)
        {
            try
            {
                var result = await _issueBiz.DeleteSubtask(subtaskId, issueId, UserId);
                return Success("Subtask deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteSubtask] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get subtask by ID
        /// </summary>
        [HttpGet("subtask/{subtaskId}")]
        public async Task<IActionResult> GetSubtaskById(long subtaskId)
        {
            try
            {
                var result = await _issueBiz.GetSubtaskById(subtaskId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetSubtaskById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all subtasks for an issue
        /// </summary>
        [HttpGet("{issueId}/subtasks")]
        public async Task<IActionResult> GetSubtasksByIssueId(long issueId)
        {
            try
            {
                var result = await _issueBiz.GetSubtasksByIssueId(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetSubtasksByIssueId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Reorder subtasks
        /// </summary>
        [HttpPut("{issueId}/subtasks/reorder")]
        public async Task<IActionResult> ReorderSubtasks(long issueId, [FromBody] List<long> subtaskIds)
        {
            try
            {
                var result = await _issueBiz.ReorderSubtasks(issueId, UserId, subtaskIds);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ReorderSubtasks] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Issue History

        /// <summary>
        /// Get issue history (activity log)
        /// </summary>
        [HttpGet("{issueId}/history")]
        public async Task<IActionResult> GetIssueHistory(long issueId, [FromQuery] int limit = 50)
        {
            try
            {
                var result = await _issueBiz.GetIssueHistory(issueId, UserId, limit);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueHistory] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion

        #region AI Features

        /// <summary>
        /// Get AI summary for an issue
        /// </summary>
        [HttpPost("{issueId}/ai/summary")]
        public async Task<IActionResult> GetIssueSummary(long issueId)
        {
            try
            {
                // Check access
                var issue = await _issueBiz.GetIssueById(issueId, UserId);
                if (issue == null)
                {
                    return NotFound("Issue not found");
                }

                var result = await _aiService.GetIssueSummary(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueSummary] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get AI suggestion for an issue
        /// </summary>
        [HttpPost("{issueId}/ai/suggestion")]
        public async Task<IActionResult> GetIssueSuggestion(long issueId)
        {
            try
            {
                // Check access
                var issue = await _issueBiz.GetIssueById(issueId, UserId);
                if (issue == null)
                {
                    return NotFound("Issue not found");
                }

                var result = await _aiService.GetIssueSuggestion(issueId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetIssueSuggestion] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion
    }
}

