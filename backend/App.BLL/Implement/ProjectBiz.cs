using App.BLL.Interface;
using App.DAL.Interface;
using App.Entity.DTO.Request.Project;
using App.Entity.DTO.Response.Project;
using App.Entity.Models;
using AutoMapper;

namespace App.BLL.Implement
{
    public class ProjectBiz : IProjectBiz
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public ProjectBiz(IProjectRepository projectRepository, ITeamRepository teamRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        #region Project Management

        public async Task<ProjectResponseDTO> CreateProject(long userId, CreateProjectRequestDTO dto)
        {
            var project = await _projectRepository.CreateProject(userId, dto.TeamId, dto);
            return await MapToProjectResponse(project, userId);
        }

        public async Task<ProjectResponseDTO> UpdateProject(long projectId, long userId, UpdateProjectRequestDTO dto)
        {
            var project = await _projectRepository.UpdateProject(projectId, userId, dto);
            return await MapToProjectResponse(project, userId);
        }

        public async Task<bool> DeleteProject(long projectId, long userId)
        {
            return await _projectRepository.DeleteProject(projectId, userId);
        }

        public async Task<ProjectResponseDTO> GetProjectById(long projectId, long userId)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to view this project");
            }

            return await MapToProjectResponse(project, userId);
        }

        public async Task<List<ProjectResponseDTO>> GetProjectsByTeamId(long teamId, long userId)
        {
            var projects = await _projectRepository.GetProjectsByTeamId(teamId, userId);
            var result = new List<ProjectResponseDTO>();

            foreach (var project in projects)
            {
                result.Add(await MapToProjectResponse(project, userId));
            }

            return result;
        }

        public async Task<List<ProjectResponseDTO>> GetProjectsByUserId(long userId)
        {
            var projects = await _projectRepository.GetProjectsByUserId(userId);
            var result = new List<ProjectResponseDTO>();

            foreach (var project in projects)
            {
                result.Add(await MapToProjectResponse(project, userId));
            }

            return result;
        }

        public async Task<ProjectDetailResponseDTO> GetProjectDetail(long projectId, long userId)
        {
            return await _projectRepository.GetProjectDetailById(projectId, userId);
        }

        public async Task<bool> ArchiveProject(long projectId, long userId, bool isArchived)
        {
            return await _projectRepository.ArchiveProject(projectId, userId, isArchived);
        }

        #endregion

        #region Favorite Projects

        public async Task<bool> AddFavoriteProject(long userId, long projectId)
        {
            return await _projectRepository.AddFavoriteProject(userId, projectId);
        }

        public async Task<bool> RemoveFavoriteProject(long userId, long projectId)
        {
            return await _projectRepository.RemoveFavoriteProject(userId, projectId);
        }

        public async Task<List<FavoriteProjectResponseDTO>> GetFavoriteProjects(long userId)
        {
            return await _projectRepository.GetFavoriteProjects(userId);
        }

        #endregion

        #region Helper Methods

        private async Task<ProjectResponseDTO> MapToProjectResponse(ProjectModel project, long userId)
        {
            var isFavorite = await _projectRepository.IsFavoriteProject(userId, project.Id);

            return new ProjectResponseDTO
            {
                Id = project.Id,
                TeamId = project.TeamId,
                TeamName = project.Team?.Name,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner?.Name,
                OwnerEmail = project.Owner?.Email,
                Name = project.Name,
                Description = project.Description,
                IsArchived = project.IsArchived,
                IsFavorite = isFavorite,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        #endregion
    }
}

