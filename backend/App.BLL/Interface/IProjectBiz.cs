using App.Entity.DTO.Request.Project;
using App.Entity.DTO.Response.Project;

namespace App.BLL.Interface
{
    public interface IProjectBiz
    {
        // Project Management
        Task<ProjectResponseDTO> CreateProject(long userId, CreateProjectRequestDTO dto);
        Task<ProjectResponseDTO> UpdateProject(long projectId, long userId, UpdateProjectRequestDTO dto);
        Task<bool> DeleteProject(long projectId, long userId);
        Task<ProjectResponseDTO> GetProjectById(long projectId, long userId);
        Task<List<ProjectResponseDTO>> GetProjectsByTeamId(long teamId, long userId);
        Task<List<ProjectResponseDTO>> GetProjectsByUserId(long userId);
        Task<ProjectDetailResponseDTO> GetProjectDetail(long projectId, long userId);
        Task<bool> ArchiveProject(long projectId, long userId, bool isArchived);

        // Favorite Projects
        Task<bool> AddFavoriteProject(long userId, long projectId);
        Task<bool> RemoveFavoriteProject(long userId, long projectId);
        Task<List<FavoriteProjectResponseDTO>> GetFavoriteProjects(long userId);
    }
}

