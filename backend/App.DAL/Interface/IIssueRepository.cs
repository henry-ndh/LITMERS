using App.Entity.DTO.Request.Issue;
using App.Entity.DTO.Response.Issue;
using App.Entity.Models;

namespace App.DAL.Interface
{
    public interface IIssueRepository
    {
        // Issue Status Management
        Task<IssueStatusModel> CreateIssueStatus(long projectId, CreateIssueStatusRequestDTO dto);
        Task<IssueStatusModel> UpdateIssueStatus(long statusId, long projectId, UpdateIssueStatusRequestDTO dto);
        Task<bool> DeleteIssueStatus(long statusId, long projectId);
        Task<IssueStatusModel> GetIssueStatusById(long statusId);
        Task<List<IssueStatusResponseDTO>> GetIssueStatusesByProjectId(long projectId);
        Task<bool> ReorderStatuses(long projectId, List<long> statusIds);
        Task<int> GetIssueCountByStatusId(long statusId);

        // Issue Management
        Task<IssueModel> CreateIssue(long userId, CreateIssueRequestDTO dto);
        Task<IssueModel> UpdateIssue(long issueId, long userId, UpdateIssueRequestDTO dto);
        Task<bool> DeleteIssue(long issueId, long userId);
        Task<IssueModel> GetIssueById(long issueId);
        Task<List<IssueResponseDTO>> GetIssuesByProjectId(long projectId, long userId);
        Task<List<IssueResponseDTO>> GetIssuesByStatusId(long statusId, long userId);
        Task<IssueDetailResponseDTO> GetIssueDetailById(long issueId, long userId);
        Task<bool> MoveIssue(long issueId, long userId, MoveIssueRequestDTO dto);
        Task<bool> HasIssueAccess(long issueId, long userId);

        // Project Label Management
        Task<ProjectLabelModel> CreateProjectLabel(long projectId, CreateProjectLabelRequestDTO dto);
        Task<ProjectLabelModel> UpdateProjectLabel(long labelId, long projectId, UpdateProjectLabelRequestDTO dto);
        Task<bool> DeleteProjectLabel(long labelId, long projectId);
        Task<ProjectLabelModel> GetProjectLabelById(long labelId);
        Task<List<LabelResponseDTO>> GetProjectLabels(long projectId);

        // Issue Label Management
        Task<bool> AddLabelToIssue(long issueId, long labelId);
        Task<bool> RemoveLabelFromIssue(long issueId, long labelId);
        Task<bool> UpdateIssueLabels(long issueId, List<long> labelIds);
        Task<List<LabelResponseDTO>> GetIssueLabels(long issueId);

        // Subtask Management
        Task<IssueSubtaskModel> CreateSubtask(long issueId, CreateSubtaskRequestDTO dto);
        Task<IssueSubtaskModel> UpdateSubtask(long subtaskId, long issueId, UpdateSubtaskRequestDTO dto);
        Task<bool> DeleteSubtask(long subtaskId, long issueId);
        Task<IssueSubtaskModel> GetSubtaskById(long subtaskId);
        Task<List<SubtaskResponseDTO>> GetSubtasksByIssueId(long issueId);
        Task<bool> ReorderSubtasks(long issueId, List<long> subtaskIds);

        // Issue History
        Task<IssueHistoryModel> CreateHistory(long issueId, long actorId, string field, string? oldValue, string? newValue);
        Task<List<IssueHistoryResponseDTO>> GetIssueHistory(long issueId, int limit = 50);
    }
}

