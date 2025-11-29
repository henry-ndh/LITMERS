using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request.Issue;
using App.Entity.DTO.Response.Issue;
using App.Entity.Models;
using App.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Implement
{
    public class IssueRepository : AppBaseRepository, IIssueRepository
    {
        private readonly BaseDBContext _dbContext;
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationRepository _notificationRepository;

        public IssueRepository(BaseDBContext dbContext, IProjectRepository projectRepository, INotificationRepository notificationRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _projectRepository = projectRepository;
            _notificationRepository = notificationRepository;
        }

        #region Issue Status Management

        public async Task<IssueStatusModel> CreateIssueStatus(long projectId, CreateIssueStatusRequestDTO dto)
        {
            // Check if project exists and user has access
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check if status name already exists in project
            var existingStatus = await _dbContext.Set<IssueStatusModel>()
                .Where(s => s.ProjectId == projectId && s.Name == dto.Name && s.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (existingStatus != null)
            {
                throw new Exception("Status name already exists in this project");
            }

            // If this is default, unset other defaults
            if (dto.IsDefault)
            {
                var currentDefaults = await _dbContext.Set<IssueStatusModel>()
                    .Where(s => s.ProjectId == projectId && s.IsDefault && s.DeletedAt == null)
                    .ToListAsync();

                foreach (var defaultStatus in currentDefaults)
                {
                    defaultStatus.IsDefault = false;
                }
                _dbContext.Set<IssueStatusModel>().UpdateRange(currentDefaults);
            }

            var status = new IssueStatusModel
            {
                ProjectId = projectId,
                Name = dto.Name,
                Color = dto.Color,
                Position = dto.Position,
                IsDefault = dto.IsDefault,
                WipLimit = dto.WipLimit,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<IssueStatusModel>().AddAsync(status);
            await _dbContext.SaveChangesAsync();

            return status;
        }

        public async Task<IssueStatusModel> UpdateIssueStatus(long statusId, long projectId, UpdateIssueStatusRequestDTO dto)
        {
            var status = await _dbContext.Set<IssueStatusModel>()
                .Where(s => s.Id == statusId && s.ProjectId == projectId && s.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (status == null)
            {
                throw new Exception("Issue status not found");
            }

            // Check if new name conflicts with existing status
            if (status.Name != dto.Name)
            {
                var existingStatus = await _dbContext.Set<IssueStatusModel>()
                    .Where(s => s.ProjectId == projectId && s.Name == dto.Name && s.Id != statusId && s.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (existingStatus != null)
                {
                    throw new Exception("Status name already exists in this project");
                }
            }

            // If setting as default, unset other defaults
            if (dto.IsDefault && !status.IsDefault)
            {
                var currentDefaults = await _dbContext.Set<IssueStatusModel>()
                    .Where(s => s.ProjectId == projectId && s.IsDefault && s.Id != statusId && s.DeletedAt == null)
                    .ToListAsync();

                foreach (var defaultStatus in currentDefaults)
                {
                    defaultStatus.IsDefault = false;
                }
                _dbContext.Set<IssueStatusModel>().UpdateRange(currentDefaults);
            }

            status.Name = dto.Name;
            status.Color = dto.Color;
            status.Position = dto.Position;
            status.IsDefault = dto.IsDefault;
            status.WipLimit = dto.WipLimit;
            status.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<IssueStatusModel>().Update(status);
            await _dbContext.SaveChangesAsync();

            return status;
        }

        public async Task<bool> DeleteIssueStatus(long statusId, long projectId)
        {
            var status = await _dbContext.Set<IssueStatusModel>()
                .Where(s => s.Id == statusId && s.ProjectId == projectId && s.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (status == null)
            {
                throw new Exception("Issue status not found");
            }

            // Check if status has issues
            var hasIssues = await _dbContext.Set<IssueModel>()
                .AnyAsync(i => i.StatusId == statusId && i.DeletedAt == null);

            if (hasIssues)
            {
                throw new Exception("Cannot delete status that has issues. Please move or delete issues first.");
            }

            status.DeletedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<IssueStatusModel>().Update(status);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IssueStatusModel> GetIssueStatusById(long statusId)
        {
            return await _dbContext.Set<IssueStatusModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Where(s => s.Id == statusId && s.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<IssueStatusResponseDTO>> GetIssueStatusesByProjectId(long projectId)
        {
            return await _dbContext.Set<IssueStatusModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Where(s => s.ProjectId == projectId && s.DeletedAt == null)
                .OrderBy(s => s.Position)
                .Select(s => new IssueStatusResponseDTO
                {
                    Id = s.Id,
                    ProjectId = s.ProjectId,
                    Name = s.Name,
                    Color = s.Color,
                    Position = s.Position,
                    IsDefault = s.IsDefault,
                    WipLimit = s.WipLimit,
                    IssueCount = _dbContext.Set<IssueModel>()
                        .Count(i => i.StatusId == s.Id && i.DeletedAt == null),
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<int> GetIssueCountByStatusId(long statusId)
        {
            return await _dbContext.Set<IssueModel>()
                .CountAsync(i => i.StatusId == statusId && i.DeletedAt == null);
        }

        public async Task<bool> ReorderStatuses(long projectId, List<long> statusIds)
        {
            var statuses = await _dbContext.Set<IssueStatusModel>()
                .Where(s => s.ProjectId == projectId && statusIds.Contains(s.Id) && s.DeletedAt == null)
                .ToListAsync();

            if (statuses.Count != statusIds.Count)
            {
                throw new Exception("Some statuses not found");
            }

            for (int i = 0; i < statusIds.Count; i++)
            {
                var status = statuses.First(s => s.Id == statusIds[i]);
                status.Position = i;
                status.UpdatedAt = Utils.GetCurrentVNTime();
            }

            _dbContext.Set<IssueStatusModel>().UpdateRange(statuses);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Issue Management

        public async Task<IssueModel> CreateIssue(long userId, CreateIssueRequestDTO dto)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(dto.ProjectId, userId))
            {
                throw new Exception("You don't have permission to create issues in this project");
            }

            // Check if status exists and belongs to project
            var status = await GetIssueStatusById(dto.StatusId);
            if (status == null || status.ProjectId != dto.ProjectId)
            {
                throw new Exception("Invalid status for this project");
            }

            var issue = new IssueModel
            {
                ProjectId = dto.ProjectId,
                StatusId = dto.StatusId,
                Title = dto.Title,
                Description = dto.Description,
                OwnerId = userId,
                AssigneeId = dto.AssigneeId,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Position = dto.Position,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<IssueModel>().AddAsync(issue);
            await _dbContext.SaveChangesAsync();

            // Notify assignee if assigned
            if (dto.AssigneeId.HasValue && dto.AssigneeId.Value != userId)
            {
                var assignee = await _dbContext.Users
                    .AsNoTracking() // No tracking for read-only
                    .FirstOrDefaultAsync(u => u.Id == dto.AssigneeId.Value && u.IsActive);
                if (assignee != null)
                {
                    var payload = System.Text.Json.JsonSerializer.Serialize(new { issueId = issue.Id, issueTitle = issue.Title });
                    await _notificationRepository.CreateNotification(
                        assignee.Id,
                        App.Entity.Models.Enums.NotificationType.ISSUE_ASSIGNED,
                        $"You have been assigned to issue: {issue.Title}",
                        $"Issue '{issue.Title}' has been assigned to you.",
                        payload
                    );
                }
            }

            // Add labels if provided
            if (dto.LabelIds != null && dto.LabelIds.Any())
            {
                // Check max labels per issue (5)
                if (dto.LabelIds.Count > 5)
                {
                    throw new Exception("Maximum 5 labels per issue allowed");
                }
                await UpdateIssueLabels(issue.Id, dto.LabelIds);
            }

            // Create history for issue creation
            await CreateHistory(issue.Id, userId, "created", null, $"Created issue '{issue.Title}'");

            return issue;
        }

        public async Task<IssueModel> UpdateIssue(long issueId, long userId, UpdateIssueRequestDTO dto)
        {
            // Get issue WITH tracking for update (not using GetIssueById which has AsNoTracking)
            var issue = await _dbContext.Set<IssueModel>()
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Where(i => i.Id == issueId && i.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            // Check access
            if (!await HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to update this issue");
            }

            // Track changes for history
            if (dto.StatusId.HasValue && dto.StatusId.Value != issue.StatusId)
            {
                var oldStatus = await GetIssueStatusById(issue.StatusId);
                var newStatus = await GetIssueStatusById(dto.StatusId.Value);
                if (newStatus == null || newStatus.ProjectId != issue.ProjectId)
                {
                    throw new Exception("Invalid status for this project");
                }
                await CreateHistory(issueId, userId, "status", oldStatus?.Name, newStatus?.Name);
                issue.StatusId = dto.StatusId.Value;
            }

            if (!string.IsNullOrEmpty(dto.Title) && dto.Title != issue.Title)
            {
                await CreateHistory(issueId, userId, "title", issue.Title, dto.Title);
                issue.Title = dto.Title;
            }

            if (dto.Description != null && dto.Description != issue.Description)
            {
                issue.Description = dto.Description;
            }

            if (dto.AssigneeId.HasValue && dto.AssigneeId.Value != issue.AssigneeId)
            {
                var oldAssignee = issue.AssigneeId.HasValue 
                    ? await _dbContext.Users
                        .AsNoTracking() // No tracking for read-only
                        .FirstOrDefaultAsync(u => u.Id == issue.AssigneeId.Value && u.IsActive)
                    : null;
                var newAssignee = await _dbContext.Users
                    .AsNoTracking() // No tracking for read-only
                    .FirstOrDefaultAsync(u => u.Id == dto.AssigneeId.Value && u.IsActive);
                await CreateHistory(issueId, userId, "assignee", oldAssignee?.Name, newAssignee?.Name);
                issue.AssigneeId = dto.AssigneeId;

                // Notify new assignee
                if (newAssignee != null && newAssignee.Id != userId)
                {
                    var payload = System.Text.Json.JsonSerializer.Serialize(new { issueId = issueId, issueTitle = issue.Title });
                    await _notificationRepository.CreateNotification(
                        newAssignee.Id,
                        App.Entity.Models.Enums.NotificationType.ISSUE_ASSIGNED,
                        $"You have been assigned to issue: {issue.Title}",
                        $"Issue '{issue.Title}' has been assigned to you.",
                        payload
                    );
                }
            }

            if (dto.DueDate.HasValue && dto.DueDate.Value != issue.DueDate)
            {
                await CreateHistory(issueId, userId, "due_date", 
                    issue.DueDate?.ToString("yyyy-MM-dd"), 
                    dto.DueDate.Value.ToString("yyyy-MM-dd"));
                issue.DueDate = dto.DueDate;
            }

            if (dto.Priority.HasValue && dto.Priority.Value != issue.Priority)
            {
                await CreateHistory(issueId, userId, "priority", issue.Priority.ToString(), dto.Priority.Value.ToString());
                issue.Priority = dto.Priority.Value;
            }

            if (dto.Position.HasValue)
            {
                issue.Position = dto.Position.Value;
            }

            issue.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<IssueModel>().Update(issue);
            await _dbContext.SaveChangesAsync();

            // Update labels if provided
            if (dto.LabelIds != null)
            {
                // Check max labels per issue (5)
                if (dto.LabelIds.Count > 5)
                {
                    throw new Exception("Maximum 5 labels per issue allowed");
                }
                await UpdateIssueLabels(issueId, dto.LabelIds);
            }

            return issue;
        }

        public async Task<bool> DeleteIssue(long issueId, long userId)
        {
            var issue = await GetIssueById(issueId);
            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            // Only owner can delete
            if (issue.OwnerId != userId)
            {
                throw new Exception("Only issue owner can delete the issue");
            }

            issue.DeletedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<IssueModel>().Update(issue);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IssueModel> GetIssueById(long issueId)
        {
            return await _dbContext.Set<IssueModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.Owner)
                .Include(i => i.Assignee)
                .Where(i => i.Id == issueId && i.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<IssueResponseDTO>> GetIssuesByProjectId(long projectId, long userId)
        {
            // Check access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to view issues in this project");
            }

            // Load issues first, then order in memory to avoid SQL issues with navigation properties
            var issues = await _dbContext.Set<IssueModel>()
                .AsNoTracking()
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.Owner)
                .Include(i => i.Assignee)
                .Where(i => i.ProjectId == projectId && i.DeletedAt == null)
                .ToListAsync();

            // Order in memory after loading to avoid SQL generation issues
            issues = issues
                .OrderBy(i => i.Status?.Position ?? 0)
                .ThenBy(i => i.Position)
                .ToList();

            var issueIds = issues.Select(i => i.Id).ToList();

            // Batch load labels, subtasks, comments - load all then group in memory to avoid SQL issues
            var allLabels = await _dbContext.Set<IssueLabelModel>()
                .AsNoTracking()
                .Include(il => il.Label)
                .Where(il => issueIds.Contains(il.IssueId))
                .ToListAsync();

            var labelsDict = allLabels
                .GroupBy(il => il.IssueId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(il => new LabelResponseDTO
                    {
                        Id = il.Label.Id,
                        ProjectId = il.Label.ProjectId,
                        Name = il.Label.Name,
                        Color = il.Label.Color,
                        CreatedAt = il.Label.CreatedAt,
                        UpdatedAt = il.Label.UpdatedAt
                    }).ToList()
                );

            var allSubtasks = await _dbContext.Set<IssueSubtaskModel>()
                .AsNoTracking()
                .Where(st => issueIds.Contains(st.IssueId))
                .ToListAsync();

            var subtaskCounts = allSubtasks
                .GroupBy(st => st.IssueId)
                .ToDictionary(
                    g => g.Key,
                    g => new { Total = g.Count(), Completed = g.Count(st => st.IsDone) }
                );

            var allComments = await _dbContext.Set<CommentModel>()
                .AsNoTracking()
                .Where(c => issueIds.Contains(c.IssueId) && c.DeletedAt == null)
                .ToListAsync();

            var commentCounts = allComments
                .GroupBy(c => c.IssueId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Map to DTOs
            return issues.Select(i => new IssueResponseDTO
            {
                Id = i.Id,
                ProjectId = i.ProjectId,
                ProjectName = i.Project.Name,
                StatusId = i.StatusId,
                StatusName = i.Status.Name,
                StatusColor = i.Status.Color,
                Title = i.Title,
                Description = i.Description,
                OwnerId = i.OwnerId,
                OwnerName = i.Owner.Name,
                OwnerEmail = i.Owner.Email,
                AssigneeId = i.AssigneeId,
                AssigneeName = i.Assignee != null ? i.Assignee.Name : null,
                AssigneeEmail = i.Assignee != null ? i.Assignee.Email : null,
                DueDate = i.DueDate,
                Priority = i.Priority,
                Position = i.Position,
                Labels = labelsDict.GetValueOrDefault(i.Id, new List<LabelResponseDTO>()),
                SubtaskCount = subtaskCounts.GetValueOrDefault(i.Id)?.Total ?? 0,
                CompletedSubtaskCount = subtaskCounts.GetValueOrDefault(i.Id)?.Completed ?? 0,
                CommentCount = commentCounts.GetValueOrDefault(i.Id, 0),
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).ToList();
        }

        public async Task<List<IssueResponseDTO>> GetIssuesByStatusId(long statusId, long userId)
        {
            var status = await GetIssueStatusById(statusId);
            if (status == null)
            {
                throw new Exception("Status not found");
            }

            // Check project access
            if (!await _projectRepository.HasProjectAccess(status.ProjectId, userId))
            {
                throw new Exception("You don't have permission to view issues in this project");
            }

            // Performance: Load labels, subtasks, comments in batch to avoid N+1 queries
            var issues = await _dbContext.Set<IssueModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.Owner)
                .Include(i => i.Assignee)
                .Where(i => i.StatusId == statusId && i.DeletedAt == null)
                .OrderBy(i => i.Position)
                .ToListAsync();

            var issueIds = issues.Select(i => i.Id).ToList();

            // Batch load labels, subtasks, comments - load all then group in memory to avoid SQL issues
            var allLabels = await _dbContext.Set<IssueLabelModel>()
                .AsNoTracking()
                .Include(il => il.Label)
                .Where(il => issueIds.Contains(il.IssueId))
                .ToListAsync();

            var labelsDict = allLabels
                .GroupBy(il => il.IssueId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(il => new LabelResponseDTO
                    {
                        Id = il.Label.Id,
                        ProjectId = il.Label.ProjectId,
                        Name = il.Label.Name,
                        Color = il.Label.Color,
                        CreatedAt = il.Label.CreatedAt,
                        UpdatedAt = il.Label.UpdatedAt
                    }).ToList()
                );

            var allSubtasks = await _dbContext.Set<IssueSubtaskModel>()
                .AsNoTracking()
                .Where(st => issueIds.Contains(st.IssueId))
                .ToListAsync();

            var subtaskCounts = allSubtasks
                .GroupBy(st => st.IssueId)
                .ToDictionary(
                    g => g.Key,
                    g => new { Total = g.Count(), Completed = g.Count(st => st.IsDone) }
                );

            var allComments = await _dbContext.Set<CommentModel>()
                .AsNoTracking()
                .Where(c => issueIds.Contains(c.IssueId) && c.DeletedAt == null)
                .ToListAsync();

            var commentCounts = allComments
                .GroupBy(c => c.IssueId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Map to DTOs
            return issues.Select(i => new IssueResponseDTO
            {
                Id = i.Id,
                ProjectId = i.ProjectId,
                ProjectName = i.Project.Name,
                StatusId = i.StatusId,
                StatusName = i.Status.Name,
                StatusColor = i.Status.Color,
                Title = i.Title,
                Description = i.Description,
                OwnerId = i.OwnerId,
                OwnerName = i.Owner.Name,
                OwnerEmail = i.Owner.Email,
                AssigneeId = i.AssigneeId,
                AssigneeName = i.Assignee != null ? i.Assignee.Name : null,
                AssigneeEmail = i.Assignee != null ? i.Assignee.Email : null,
                DueDate = i.DueDate,
                Priority = i.Priority,
                Position = i.Position,
                Labels = labelsDict.GetValueOrDefault(i.Id, new List<LabelResponseDTO>()),
                SubtaskCount = subtaskCounts.GetValueOrDefault(i.Id)?.Total ?? 0,
                CompletedSubtaskCount = subtaskCounts.GetValueOrDefault(i.Id)?.Completed ?? 0,
                CommentCount = commentCounts.GetValueOrDefault(i.Id, 0),
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).ToList();
        }

        public async Task<IssueDetailResponseDTO> GetIssueDetailById(long issueId, long userId)
        {
            var issue = await GetIssueById(issueId);
            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            // Check access
            if (!await HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to view this issue");
            }

            var labels = await GetIssueLabels(issueId);
            var subtasks = await GetSubtasksByIssueId(issueId);
            var commentCount = await _dbContext.Set<CommentModel>()
                .CountAsync(c => c.IssueId == issueId && c.DeletedAt == null);

            return new IssueDetailResponseDTO
            {
                Id = issue.Id,
                ProjectId = issue.ProjectId,
                ProjectName = issue.Project?.Name,
                StatusId = issue.StatusId,
                StatusName = issue.Status?.Name,
                StatusColor = issue.Status?.Color,
                Title = issue.Title,
                Description = issue.Description,
                OwnerId = issue.OwnerId,
                OwnerName = issue.Owner?.Name,
                OwnerEmail = issue.Owner?.Email,
                AssigneeId = issue.AssigneeId,
                AssigneeName = issue.Assignee?.Name,
                AssigneeEmail = issue.Assignee?.Email,
                DueDate = issue.DueDate,
                Priority = issue.Priority,
                Position = issue.Position,
                Labels = labels,
                Subtasks = subtasks,
                CommentCount = commentCount,
                CreatedAt = issue.CreatedAt,
                UpdatedAt = issue.UpdatedAt
            };
        }

        public async Task<bool> MoveIssue(long issueId, long userId, MoveIssueRequestDTO dto)
        {
            // Get issue WITH tracking for update (not using GetIssueById which has AsNoTracking)
            var issue = await _dbContext.Set<IssueModel>()
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Where(i => i.Id == issueId && i.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            // Check access
            if (!await HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to move this issue");
            }

            // Check if new status exists and belongs to same project
            var newStatus = await GetIssueStatusById(dto.StatusId);
            if (newStatus == null || newStatus.ProjectId != issue.ProjectId)
            {
                throw new Exception("Invalid status for this project");
            }

            // FR-054: Check WIP Limit if status has one
            if (newStatus.WipLimit.HasValue)
            {
                var currentIssueCount = await _dbContext.Set<IssueModel>()
                    .CountAsync(i => i.StatusId == dto.StatusId && i.DeletedAt == null && i.Id != issueId);
                
                if (currentIssueCount >= newStatus.WipLimit.Value)
                {
                    throw new Exception($"WIP Limit exceeded. Maximum {newStatus.WipLimit.Value} issues allowed in status '{newStatus.Name}'");
                }
            }

            var oldStatusId = issue.StatusId;
            issue.StatusId = dto.StatusId;
            issue.Position = dto.Position;
            issue.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<IssueModel>().Update(issue);
            await _dbContext.SaveChangesAsync();

            // Create history if status changed
            if (oldStatusId != dto.StatusId)
            {
                var oldStatus = await GetIssueStatusById(oldStatusId);
                await CreateHistory(issueId, userId, "status", oldStatus?.Name, newStatus?.Name);
            }

            return true;
        }

        public async Task<bool> HasIssueAccess(long issueId, long userId)
        {
            var issue = await GetIssueById(issueId);
            if (issue == null) return false;

            return await _projectRepository.HasProjectAccess(issue.ProjectId, userId);
        }

        #endregion

        #region Project Label Management

        public async Task<ProjectLabelModel> CreateProjectLabel(long projectId, CreateProjectLabelRequestDTO dto)
        {
            // Check if project exists
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            // Check max labels per project (20)
            var currentLabelCount = await _dbContext.Set<ProjectLabelModel>()
                .CountAsync(l => l.ProjectId == projectId);

            if (currentLabelCount >= 20)
            {
                throw new Exception("Maximum 20 labels per project allowed");
            }

            // Check if label name already exists in project
            var existingLabel = await _dbContext.Set<ProjectLabelModel>()
                .Where(l => l.ProjectId == projectId && l.Name == dto.Name)
                .FirstOrDefaultAsync();

            if (existingLabel != null)
            {
                throw new Exception("Label name already exists in this project");
            }

            var label = new ProjectLabelModel
            {
                ProjectId = projectId,
                Name = dto.Name,
                Color = dto.Color,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<ProjectLabelModel>().AddAsync(label);
            await _dbContext.SaveChangesAsync();

            return label;
        }

        public async Task<ProjectLabelModel> UpdateProjectLabel(long labelId, long projectId, UpdateProjectLabelRequestDTO dto)
        {
            var label = await _dbContext.Set<ProjectLabelModel>()
                .Where(l => l.Id == labelId && l.ProjectId == projectId)
                .FirstOrDefaultAsync();

            if (label == null)
            {
                throw new Exception("Label not found");
            }

            // Check if new name conflicts
            if (label.Name != dto.Name)
            {
                var existingLabel = await _dbContext.Set<ProjectLabelModel>()
                    .Where(l => l.ProjectId == projectId && l.Name == dto.Name && l.Id != labelId)
                    .FirstOrDefaultAsync();

                if (existingLabel != null)
                {
                    throw new Exception("Label name already exists in this project");
                }
            }

            label.Name = dto.Name;
            label.Color = dto.Color;
            label.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<ProjectLabelModel>().Update(label);
            await _dbContext.SaveChangesAsync();

            return label;
        }

        public async Task<bool> DeleteProjectLabel(long labelId, long projectId)
        {
            var label = await _dbContext.Set<ProjectLabelModel>()
                .Where(l => l.Id == labelId && l.ProjectId == projectId)
                .FirstOrDefaultAsync();

            if (label == null)
            {
                throw new Exception("Label not found");
            }

            // Remove all issue-label associations
            var issueLabels = await _dbContext.Set<IssueLabelModel>()
                .Where(il => il.LabelId == labelId)
                .ToListAsync();

            _dbContext.Set<IssueLabelModel>().RemoveRange(issueLabels);
            _dbContext.Set<ProjectLabelModel>().Remove(label);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ProjectLabelModel> GetProjectLabelById(long labelId)
        {
            return await _dbContext.Set<ProjectLabelModel>()
                .Where(l => l.Id == labelId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LabelResponseDTO>> GetProjectLabels(long projectId)
        {
            return await _dbContext.Set<ProjectLabelModel>()
                .Where(l => l.ProjectId == projectId)
                .OrderBy(l => l.Name)
                .Select(l => new LabelResponseDTO
                {
                    Id = l.Id,
                    ProjectId = l.ProjectId,
                    Name = l.Name,
                    Color = l.Color,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToListAsync();
        }

        #endregion

        #region Issue Label Management

        public async Task<bool> AddLabelToIssue(long issueId, long labelId)
        {
            // Check if already exists
            var existing = await _dbContext.Set<IssueLabelModel>()
                .Where(il => il.IssueId == issueId && il.LabelId == labelId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return true; // Already added
            }

            // Check max labels per issue (5)
            var currentLabelCount = await _dbContext.Set<IssueLabelModel>()
                .CountAsync(il => il.IssueId == issueId);

            if (currentLabelCount >= 5)
            {
                throw new Exception("Maximum 5 labels per issue allowed");
            }

            // Verify label belongs to same project as issue
            var issue = await GetIssueById(issueId);
            var label = await GetProjectLabelById(labelId);

            if (issue == null || label == null || issue.ProjectId != label.ProjectId)
            {
                throw new Exception("Label does not belong to the same project as the issue");
            }

            var issueLabel = new IssueLabelModel
            {
                IssueId = issueId,
                LabelId = labelId
            };

            await _dbContext.Set<IssueLabelModel>().AddAsync(issueLabel);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveLabelFromIssue(long issueId, long labelId)
        {
            var issueLabel = await _dbContext.Set<IssueLabelModel>()
                .Where(il => il.IssueId == issueId && il.LabelId == labelId)
                .FirstOrDefaultAsync();

            if (issueLabel == null)
            {
                return false;
            }

            _dbContext.Set<IssueLabelModel>().Remove(issueLabel);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateIssueLabels(long issueId, List<long> labelIds)
        {
            // Check max labels per issue (5)
            if (labelIds != null && labelIds.Count > 5)
            {
                throw new Exception("Maximum 5 labels per issue allowed");
            }

            // Get current labels
            var currentLabels = await _dbContext.Set<IssueLabelModel>()
                .Where(il => il.IssueId == issueId)
                .ToListAsync();

            var currentLabelIds = currentLabels.Select(cl => cl.LabelId).ToList();
            var labelsToAdd = labelIds != null ? labelIds.Except(currentLabelIds).ToList() : new List<long>();
            var labelsToRemove = currentLabelIds.Except(labelIds ?? new List<long>()).ToList();

            // Remove labels
            if (labelsToRemove.Any())
            {
                var toRemove = currentLabels.Where(cl => labelsToRemove.Contains(cl.LabelId)).ToList();
                _dbContext.Set<IssueLabelModel>().RemoveRange(toRemove);
            }

            // Add new labels
            if (labelsToAdd.Any())
            {
                var issue = await GetIssueById(issueId);
                var labels = await _dbContext.Set<ProjectLabelModel>()
                    .Where(l => labelsToAdd.Contains(l.Id) && l.ProjectId == issue.ProjectId)
                    .ToListAsync();

                if (labels.Count != labelsToAdd.Count)
                {
                    throw new Exception("Some labels not found or don't belong to the project");
                }

                var issueLabels = labelsToAdd.Select(labelId => new IssueLabelModel
                {
                    IssueId = issueId,
                    LabelId = labelId
                }).ToList();

                await _dbContext.Set<IssueLabelModel>().AddRangeAsync(issueLabels);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<LabelResponseDTO>> GetIssueLabels(long issueId)
        {
            return await _dbContext.Set<IssueLabelModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(il => il.Label)
                .Where(il => il.IssueId == issueId)
                .Select(il => new LabelResponseDTO
                {
                    Id = il.Label.Id,
                    ProjectId = il.Label.ProjectId,
                    Name = il.Label.Name,
                    Color = il.Label.Color,
                    CreatedAt = il.Label.CreatedAt,
                    UpdatedAt = il.Label.UpdatedAt
                })
                .ToListAsync();
        }

        #endregion

        #region Subtask Management

        public async Task<IssueSubtaskModel> CreateSubtask(long issueId, CreateSubtaskRequestDTO dto)
        {
            var issue = await GetIssueById(issueId);
            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            var subtask = new IssueSubtaskModel
            {
                IssueId = issueId,
                Title = dto.Title,
                IsDone = false,
                Position = dto.Position,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<IssueSubtaskModel>().AddAsync(subtask);
            await _dbContext.SaveChangesAsync();

            return subtask;
        }

        public async Task<IssueSubtaskModel> UpdateSubtask(long subtaskId, long issueId, UpdateSubtaskRequestDTO dto)
        {
            var subtask = await _dbContext.Set<IssueSubtaskModel>()
                .Where(st => st.Id == subtaskId && st.IssueId == issueId)
                .FirstOrDefaultAsync();

            if (subtask == null)
            {
                throw new Exception("Subtask not found");
            }

            if (!string.IsNullOrEmpty(dto.Title))
            {
                subtask.Title = dto.Title;
            }

            if (dto.IsDone.HasValue)
            {
                subtask.IsDone = dto.IsDone.Value;
            }

            if (dto.Position.HasValue)
            {
                subtask.Position = dto.Position.Value;
            }

            // Handle assigneeId - can be set to null to unassign
            // If AssigneeId is provided (even if null), update it
            if (dto.AssigneeId.HasValue)
            {
                if (dto.AssigneeId.Value == 0)
                {
                    subtask.AssigneeId = null; // Unassign
                }
                else
                {
                    subtask.AssigneeId = dto.AssigneeId.Value;
                }
            }

            subtask.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<IssueSubtaskModel>().Update(subtask);
            await _dbContext.SaveChangesAsync();

            return subtask;
        }

        public async Task<bool> DeleteSubtask(long subtaskId, long issueId)
        {
            var subtask = await _dbContext.Set<IssueSubtaskModel>()
                .Where(st => st.Id == subtaskId && st.IssueId == issueId)
                .FirstOrDefaultAsync();

            if (subtask == null)
            {
                throw new Exception("Subtask not found");
            }

            _dbContext.Set<IssueSubtaskModel>().Remove(subtask);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IssueSubtaskModel> GetSubtaskById(long subtaskId)
        {
            return await _dbContext.Set<IssueSubtaskModel>()
                .Where(st => st.Id == subtaskId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SubtaskResponseDTO>> GetSubtasksByIssueId(long issueId)
        {
            return await _dbContext.Set<IssueSubtaskModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(st => st.Assignee)
                .Where(st => st.IssueId == issueId)
                .OrderBy(st => st.Position)
                .Select(st => new SubtaskResponseDTO
                {
                    Id = st.Id,
                    IssueId = st.IssueId,
                    Title = st.Title,
                    IsDone = st.IsDone,
                    Position = st.Position,
                    AssigneeId = st.AssigneeId,
                    AssigneeName = st.Assignee != null ? st.Assignee.Name : null,
                    AssigneeEmail = st.Assignee != null ? st.Assignee.Email : null,
                    CreatedAt = st.CreatedAt,
                    UpdatedAt = st.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> ReorderSubtasks(long issueId, List<long> subtaskIds)
        {
            var subtasks = await _dbContext.Set<IssueSubtaskModel>()
                .Where(st => st.IssueId == issueId && subtaskIds.Contains(st.Id))
                .ToListAsync();

            if (subtasks.Count != subtaskIds.Count)
            {
                throw new Exception("Some subtasks not found");
            }

            for (int i = 0; i < subtaskIds.Count; i++)
            {
                var subtask = subtasks.First(st => st.Id == subtaskIds[i]);
                subtask.Position = i;
                subtask.UpdatedAt = Utils.GetCurrentVNTime();
            }

            _dbContext.Set<IssueSubtaskModel>().UpdateRange(subtasks);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Issue History

        public async Task<IssueHistoryModel> CreateHistory(long issueId, long actorId, string field, string? oldValue, string? newValue)
        {
            var history = new IssueHistoryModel
            {
                IssueId = issueId,
                ActorId = actorId,
                Field = field,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<IssueHistoryModel>().AddAsync(history);
            await _dbContext.SaveChangesAsync();

            return history;
        }

        public async Task<List<IssueHistoryResponseDTO>> GetIssueHistory(long issueId, int limit = 50)
        {
            return await _dbContext.Set<IssueHistoryModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(h => h.Actor)
                .Where(h => h.IssueId == issueId)
                .OrderByDescending(h => h.CreatedAt)
                .Take(limit)
                .Select(h => new IssueHistoryResponseDTO
                {
                    Id = h.Id,
                    IssueId = h.IssueId,
                    Field = h.Field,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    ActorId = h.ActorId,
                    ActorName = h.Actor.Name,
                    ActorEmail = h.Actor.Email,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();
        }

        #endregion
    }
}

