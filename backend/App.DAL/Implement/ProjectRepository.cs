using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request.Project;
using App.Entity.DTO.Response.Project;
using App.Entity.Models;
using App.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class ProjectRepository : AppBaseRepository, IProjectRepository
    {
        private readonly BaseDBContext _dbContext;
        private readonly ITeamRepository _teamRepository;

        public ProjectRepository(BaseDBContext dbContext, ITeamRepository teamRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _teamRepository = teamRepository;
        }

        #region Project Management

        public async Task<ProjectModel> CreateProject(long userId, long teamId, CreateProjectRequestDTO dto)
        {
            // Check if user is member of the team
            if (!await _teamRepository.IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            var project = new ProjectModel
            {
                TeamId = teamId,
                OwnerId = userId,
                Name = dto.Name,
                Description = dto.Description,
                IsArchived = false,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<ProjectModel>().AddAsync(project);
            await _dbContext.SaveChangesAsync();

            // Log activity in team
            await _teamRepository.CreateActivityLog(teamId, userId, ActivityActionType.PROJECT_CREATED, project.Id, "Project", $"Created project '{project.Name}'");

            return project;
        }

        public async Task<ProjectModel> UpdateProject(long projectId, long userId, UpdateProjectRequestDTO dto)
        {
            var project = await GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check access
            if (!await HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to update this project");
            }

            var oldName = project.Name;
            project.Name = dto.Name;
            project.Description = dto.Description;
            project.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<ProjectModel>().Update(project);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await _teamRepository.CreateActivityLog(project.TeamId, userId, ActivityActionType.PROJECT_UPDATED, projectId, "Project", $"Updated project '{oldName}' to '{project.Name}'");

            return project;
        }

        public async Task<bool> DeleteProject(long projectId, long userId)
        {
            var project = await GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Only owner can delete project
            if (!await IsProjectOwner(projectId, userId))
            {
                throw new Exception("Only project owner can delete the project");
            }

            project.DeletedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<ProjectModel>().Update(project);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await _teamRepository.CreateActivityLog(project.TeamId, userId, ActivityActionType.PROJECT_DELETED, projectId, "Project", $"Deleted project '{project.Name}'");

            return true;
        }

        public async Task<ProjectModel> GetProjectById(long projectId)
        {
            return await _dbContext.Set<ProjectModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(p => p.Team)
                .Include(p => p.Owner)
                .Where(p => p.Id == projectId && p.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProjectModel>> GetProjectsByTeamId(long teamId, long userId)
        {
            // Check if user is member of the team
            if (!await _teamRepository.IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            return await _dbContext.Set<ProjectModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(p => p.Team)
                .Include(p => p.Owner)
                .Where(p => p.TeamId == teamId && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ProjectModel>> GetProjectsByUserId(long userId)
        {
            // Performance: Join directly instead of 2 separate queries
            return await _dbContext.Set<ProjectModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(p => p.Team)
                    .ThenInclude(t => t.TeamMembers)
                .Include(p => p.Owner)
                .Where(p => p.Team.TeamMembers.Any(tm => tm.UserId == userId) && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProjectDetailResponseDTO> GetProjectDetailById(long projectId, long userId)
        {
            var project = await GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check access
            if (!await HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to view this project");
            }

            var isFavorite = await IsFavoriteProject(userId, projectId);
            var issueCount = await _dbContext.Set<IssueModel>()
                .Where(i => i.ProjectId == projectId && i.DeletedAt == null)
                .CountAsync();

            return new ProjectDetailResponseDTO
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
                IssueCount = issueCount,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        public async Task<bool> ArchiveProject(long projectId, long userId, bool isArchived)
        {
            var project = await GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check access
            if (!await HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to archive/unarchive this project");
            }

            project.IsArchived = isArchived;
            project.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<ProjectModel>().Update(project);
            await _dbContext.SaveChangesAsync();

            // Log activity
            var actionType = isArchived 
                ? ActivityActionType.PROJECT_ARCHIVED 
                : ActivityActionType.PROJECT_UNARCHIVED;
            var message = isArchived 
                ? $"Archived project '{project.Name}'" 
                : $"Unarchived project '{project.Name}'";

            await _teamRepository.CreateActivityLog(project.TeamId, userId, actionType, projectId, "Project", message);

            return true;
        }

        #endregion

        #region Permission Checks

        public async Task<bool> IsProjectMember(long projectId, long userId)
        {
            var project = await GetProjectById(projectId);
            if (project == null) return false;

            return await _teamRepository.IsTeamMember(project.TeamId, userId);
        }

        public async Task<bool> IsProjectOwner(long projectId, long userId)
        {
            var project = await GetProjectById(projectId);
            if (project == null) return false;

            return project.OwnerId == userId;
        }

        public async Task<bool> HasProjectAccess(long projectId, long userId)
        {
            return await IsProjectMember(projectId, userId);
        }

        #endregion

        #region Favorite Projects

        public async Task<bool> AddFavoriteProject(long userId, long projectId)
        {
            // Check if project exists and user has access
            if (!await HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to favorite this project");
            }

            // Check if already favorite
            var existing = await _dbContext.Set<FavoriteProjectModel>()
                .Where(fp => fp.UserId == userId && fp.ProjectId == projectId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return true; // Already favorite
            }

            var favorite = new FavoriteProjectModel
            {
                UserId = userId,
                ProjectId = projectId,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<FavoriteProjectModel>().AddAsync(favorite);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFavoriteProject(long userId, long projectId)
        {
            var favorite = await _dbContext.Set<FavoriteProjectModel>()
                .Where(fp => fp.UserId == userId && fp.ProjectId == projectId)
                .FirstOrDefaultAsync();

            if (favorite == null)
            {
                return false; // Not favorite
            }

            _dbContext.Set<FavoriteProjectModel>().Remove(favorite);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsFavoriteProject(long userId, long projectId)
        {
            return await _dbContext.Set<FavoriteProjectModel>()
                .AnyAsync(fp => fp.UserId == userId && fp.ProjectId == projectId);
        }

        public async Task<List<FavoriteProjectResponseDTO>> GetFavoriteProjects(long userId)
        {
            return await _dbContext.Set<FavoriteProjectModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(fp => fp.Project)
                    .ThenInclude(p => p.Team)
                .Where(fp => fp.UserId == userId && fp.Project.DeletedAt == null)
                .OrderByDescending(fp => fp.CreatedAt)
                .Select(fp => new FavoriteProjectResponseDTO
                {
                    ProjectId = fp.ProjectId,
                    ProjectName = fp.Project.Name,
                    TeamId = fp.Project.TeamId,
                    TeamName = fp.Project.Team.Name,
                    CreatedAt = fp.CreatedAt
                })
                .ToListAsync();
        }

        #endregion
    }
}

