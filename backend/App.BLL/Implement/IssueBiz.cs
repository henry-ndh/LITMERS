using App.BLL.Interface;
using App.DAL.Interface;
using App.Entity.DTO.Request.Issue;
using App.Entity.DTO.Response.Issue;
using App.Entity.Models;
using AutoMapper;

namespace App.BLL.Implement
{
    public class IssueBiz : IIssueBiz
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public IssueBiz(IIssueRepository issueRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _issueRepository = issueRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        #region Issue Status Management

        public async Task<IssueStatusResponseDTO> CreateIssueStatus(long projectId, long userId, CreateIssueStatusRequestDTO dto)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to create statuses in this project");
            }

            var status = await _issueRepository.CreateIssueStatus(projectId, dto);
            return await MapToIssueStatusResponse(status, userId);
        }

        public async Task<IssueStatusResponseDTO> UpdateIssueStatus(long statusId, long projectId, long userId, UpdateIssueStatusRequestDTO dto)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to update statuses in this project");
            }

            var status = await _issueRepository.UpdateIssueStatus(statusId, projectId, dto);
            return await MapToIssueStatusResponse(status, userId);
        }

        public async Task<bool> DeleteIssueStatus(long statusId, long projectId, long userId)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to delete statuses in this project");
            }

            return await _issueRepository.DeleteIssueStatus(statusId, projectId);
        }

        public async Task<IssueStatusResponseDTO> GetIssueStatusById(long statusId, long userId)
        {
            var status = await _issueRepository.GetIssueStatusById(statusId);
            if (status == null)
            {
                throw new Exception("Issue status not found");
            }

            // Check project access
            if (!await _projectRepository.HasProjectAccess(status.ProjectId, userId))
            {
                throw new Exception("You don't have permission to view this status");
            }

            return await MapToIssueStatusResponse(status, userId);
        }

        public async Task<List<IssueStatusResponseDTO>> GetIssueStatusesByProjectId(long projectId, long userId)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to view statuses in this project");
            }

            return await _issueRepository.GetIssueStatusesByProjectId(projectId);
        }

        public async Task<bool> ReorderStatuses(long projectId, long userId, List<long> statusIds)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to reorder statuses in this project");
            }

            return await _issueRepository.ReorderStatuses(projectId, statusIds);
        }

        #endregion

        #region Issue Management

        public async Task<IssueResponseDTO> CreateIssue(long userId, CreateIssueRequestDTO dto)
        {
            var issue = await _issueRepository.CreateIssue(userId, dto);
            return await MapToIssueResponse(issue, userId);
        }

        public async Task<IssueResponseDTO> UpdateIssue(long issueId, long userId, UpdateIssueRequestDTO dto)
        {
            var issue = await _issueRepository.UpdateIssue(issueId, userId, dto);
            return await MapToIssueResponse(issue, userId);
        }

        public async Task<bool> DeleteIssue(long issueId, long userId)
        {
            return await _issueRepository.DeleteIssue(issueId, userId);
        }

        public async Task<IssueResponseDTO> GetIssueById(long issueId, long userId)
        {
            var issue = await _issueRepository.GetIssueById(issueId);
            if (issue == null)
            {
                throw new Exception("Issue not found");
            }

            // Check access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to view this issue");
            }

            return await MapToIssueResponse(issue, userId);
        }

        public async Task<List<IssueResponseDTO>> GetIssuesByProjectId(long projectId, long userId)
        {
            return await _issueRepository.GetIssuesByProjectId(projectId, userId);
        }

        public async Task<List<IssueResponseDTO>> GetIssuesByStatusId(long statusId, long userId)
        {
            return await _issueRepository.GetIssuesByStatusId(statusId, userId);
        }

        public async Task<IssueDetailResponseDTO> GetIssueDetail(long issueId, long userId)
        {
            return await _issueRepository.GetIssueDetailById(issueId, userId);
        }

        public async Task<bool> MoveIssue(long issueId, long userId, MoveIssueRequestDTO dto)
        {
            return await _issueRepository.MoveIssue(issueId, userId, dto);
        }

        #endregion

        #region Project Label Management

        public async Task<LabelResponseDTO> CreateProjectLabel(long projectId, long userId, CreateProjectLabelRequestDTO dto)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to create labels in this project");
            }

            var label = await _issueRepository.CreateProjectLabel(projectId, dto);
            return _mapper.Map<LabelResponseDTO>(label);
        }

        public async Task<LabelResponseDTO> UpdateProjectLabel(long labelId, long projectId, long userId, UpdateProjectLabelRequestDTO dto)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to update labels in this project");
            }

            var label = await _issueRepository.UpdateProjectLabel(labelId, projectId, dto);
            return _mapper.Map<LabelResponseDTO>(label);
        }

        public async Task<bool> DeleteProjectLabel(long labelId, long projectId, long userId)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to delete labels in this project");
            }

            return await _issueRepository.DeleteProjectLabel(labelId, projectId);
        }

        public async Task<LabelResponseDTO> GetProjectLabelById(long labelId, long userId)
        {
            var label = await _issueRepository.GetProjectLabelById(labelId);
            if (label == null)
            {
                throw new Exception("Label not found");
            }

            // Check project access
            if (!await _projectRepository.HasProjectAccess(label.ProjectId, userId))
            {
                throw new Exception("You don't have permission to view this label");
            }

            return _mapper.Map<LabelResponseDTO>(label);
        }

        public async Task<List<LabelResponseDTO>> GetProjectLabels(long projectId, long userId)
        {
            // Check project access
            if (!await _projectRepository.HasProjectAccess(projectId, userId))
            {
                throw new Exception("You don't have permission to view labels in this project");
            }

            return await _issueRepository.GetProjectLabels(projectId);
        }

        #endregion

        #region Issue Label Management

        public async Task<bool> AddLabelToIssue(long issueId, long labelId, long userId)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to modify this issue");
            }

            return await _issueRepository.AddLabelToIssue(issueId, labelId);
        }

        public async Task<bool> RemoveLabelFromIssue(long issueId, long labelId, long userId)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to modify this issue");
            }

            return await _issueRepository.RemoveLabelFromIssue(issueId, labelId);
        }

        public async Task<bool> UpdateIssueLabels(long issueId, long userId, List<long> labelIds)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to modify this issue");
            }

            return await _issueRepository.UpdateIssueLabels(issueId, labelIds);
        }

        #endregion

        #region Subtask Management

        public async Task<SubtaskResponseDTO> CreateSubtask(long issueId, long userId, CreateSubtaskRequestDTO dto)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to create subtasks for this issue");
            }

            var subtask = await _issueRepository.CreateSubtask(issueId, dto);
            return _mapper.Map<SubtaskResponseDTO>(subtask);
        }

        public async Task<SubtaskResponseDTO> UpdateSubtask(long subtaskId, long issueId, long userId, UpdateSubtaskRequestDTO dto)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to update subtasks for this issue");
            }

            var subtask = await _issueRepository.UpdateSubtask(subtaskId, issueId, dto);
            return _mapper.Map<SubtaskResponseDTO>(subtask);
        }

        public async Task<bool> DeleteSubtask(long subtaskId, long issueId, long userId)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to delete subtasks for this issue");
            }

            return await _issueRepository.DeleteSubtask(subtaskId, issueId);
        }

        public async Task<SubtaskResponseDTO> GetSubtaskById(long subtaskId, long userId)
        {
            var subtask = await _issueRepository.GetSubtaskById(subtaskId);
            if (subtask == null)
            {
                throw new Exception("Subtask not found");
            }

            // Check issue access
            if (!await _issueRepository.HasIssueAccess(subtask.IssueId, userId))
            {
                throw new Exception("You don't have permission to view this subtask");
            }

            return _mapper.Map<SubtaskResponseDTO>(subtask);
        }

        public async Task<List<SubtaskResponseDTO>> GetSubtasksByIssueId(long issueId, long userId)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to view subtasks for this issue");
            }

            return await _issueRepository.GetSubtasksByIssueId(issueId);
        }

        public async Task<bool> ReorderSubtasks(long issueId, long userId, List<long> subtaskIds)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to reorder subtasks for this issue");
            }

            return await _issueRepository.ReorderSubtasks(issueId, subtaskIds);
        }

        #endregion

        #region Issue History

        public async Task<List<IssueHistoryResponseDTO>> GetIssueHistory(long issueId, long userId, int limit = 50)
        {
            // Check issue access
            if (!await _issueRepository.HasIssueAccess(issueId, userId))
            {
                throw new Exception("You don't have permission to view history for this issue");
            }

            return await _issueRepository.GetIssueHistory(issueId, limit);
        }

        #endregion

        #region Helper Methods

        private async Task<IssueStatusResponseDTO> MapToIssueStatusResponse(IssueStatusModel status, long userId)
        {
            // Get issue count directly from DB to avoid permission check issues
            var issueCount = await _issueRepository.GetIssueCountByStatusId(status.Id);
            
            return new IssueStatusResponseDTO
            {
                Id = status.Id,
                ProjectId = status.ProjectId,
                Name = status.Name,
                Color = status.Color,
                Position = status.Position,
                IsDefault = status.IsDefault,
                WipLimit = status.WipLimit,
                IssueCount = issueCount,
                CreatedAt = status.CreatedAt,
                UpdatedAt = status.UpdatedAt
            };
        }

        private async Task<IssueResponseDTO> MapToIssueResponse(IssueModel issue, long userId)
        {
            var labels = await _issueRepository.GetIssueLabels(issue.Id);
            var subtasks = await _issueRepository.GetSubtasksByIssueId(issue.Id);

            return new IssueResponseDTO
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
                SubtaskCount = subtasks.Count,
                CompletedSubtaskCount = subtasks.Count(st => st.IsDone),
                CommentCount = 0, // Will be populated by repository query
                CreatedAt = issue.CreatedAt,
                UpdatedAt = issue.UpdatedAt
            };
        }

        #endregion
    }
}

