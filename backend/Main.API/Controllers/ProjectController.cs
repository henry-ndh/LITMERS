using App.BLL.Interface;
using App.Entity.DTO.Request.Project;
using Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : BaseAPIController
    {
        private readonly IProjectBiz _projectBiz;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectBiz projectBiz, ILogger<ProjectController> logger)
        {
            _projectBiz = projectBiz;
            _logger = logger;
        }

        #region Project Management

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _projectBiz.CreateProject(UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update project information
        /// </summary>
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(long projectId, [FromBody] UpdateProjectRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _projectBiz.UpdateProject(projectId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a project (soft delete)
        /// </summary>
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(long projectId)
        {
            try
            {
                var result = await _projectBiz.DeleteProject(projectId, UserId);
                return Success("Project deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(long projectId)
        {
            try
            {
                var result = await _projectBiz.GetProjectById(projectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetProjectById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get project detail with additional information
        /// </summary>
        [HttpGet("{projectId}/detail")]
        public async Task<IActionResult> GetProjectDetail(long projectId)
        {
            try
            {
                var result = await _projectBiz.GetProjectDetail(projectId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetProjectDetail] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all projects in a team
        /// </summary>
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetProjectsByTeamId(long teamId)
        {
            try
            {
                var result = await _projectBiz.GetProjectsByTeamId(teamId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetProjectsByTeamId] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all projects that the current user has access to
        /// </summary>
        [HttpGet("my-projects")]
        public async Task<IActionResult> GetMyProjects()
        {
            try
            {
                var result = await _projectBiz.GetProjectsByUserId(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetMyProjects] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Archive or unarchive a project
        /// </summary>
        [HttpPut("{projectId}/archive")]
        public async Task<IActionResult> ArchiveProject(long projectId, [FromBody] ArchiveProjectRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _projectBiz.ArchiveProject(projectId, UserId, dto.IsArchived);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ArchiveProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Favorite Projects

        /// <summary>
        /// Add a project to favorites
        /// </summary>
        [HttpPost("{projectId}/favorite")]
        public async Task<IActionResult> AddFavoriteProject(long projectId)
        {
            try
            {
                var result = await _projectBiz.AddFavoriteProject(UserId, projectId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[AddFavoriteProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Remove a project from favorites
        /// </summary>
        [HttpDelete("{projectId}/favorite")]
        public async Task<IActionResult> RemoveFavoriteProject(long projectId)
        {
            try
            {
                var result = await _projectBiz.RemoveFavoriteProject(UserId, projectId);
                return Success("Project removed from favorites");
            }
            catch (Exception ex)
            {
                _logger.LogError("[RemoveFavoriteProject] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get all favorite projects of the current user
        /// </summary>
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoriteProjects()
        {
            try
            {
                var result = await _projectBiz.GetFavoriteProjects(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetFavoriteProjects] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion
    }
}

