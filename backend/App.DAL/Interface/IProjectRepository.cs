using App.Entity.DTO.Request.Project;
using App.Entity.DTO.Response.Project;
using App.Entity.Models;

namespace App.DAL.Interface
{
    public interface IProjectRepository
    {
        // Project Management
        Task<ProjectModel> CreateProject(long userId, long teamId, CreateProjectRequestDTO dto);
        Task<ProjectModel> UpdateProject(long projectId, long userId, UpdateProjectRequestDTO dto);
        Task<bool> DeleteProject(long projectId, long userId);
        Task<ProjectModel> GetProjectById(long projectId);
        Task<List<ProjectModel>> GetProjectsByTeamId(long teamId, long userId);
        Task<List<ProjectModel>> GetProjectsByUserId(long userId);
        Task<ProjectDetailResponseDTO> GetProjectDetailById(long projectId, long userId);
        Task<bool> ArchiveProject(long projectId, long userId, bool isArchived);
        
        // Permission checks
        Task<bool> IsProjectMember(long projectId, long userId);
        Task<bool> IsProjectOwner(long projectId, long userId);
        Task<bool> HasProjectAccess(long projectId, long userId);

        // Favorite Projects
        Task<bool> AddFavoriteProject(long userId, long projectId);
        Task<bool> RemoveFavoriteProject(long userId, long projectId);
        Task<bool> IsFavoriteProject(long userId, long projectId);
        Task<List<FavoriteProjectResponseDTO>> GetFavoriteProjects(long userId);
    }
}

