using App.Entity.DTO.Request.Issue;
using App.Entity.DTO.Response.Issue;

namespace App.BLL.Interface
{
    public interface IIssueBiz
    {
        // Issue Status Management
        Task<IssueStatusResponseDTO> CreateIssueStatus(long projectId, long userId, CreateIssueStatusRequestDTO dto);
        Task<IssueStatusResponseDTO> UpdateIssueStatus(long statusId, long projectId, long userId, UpdateIssueStatusRequestDTO dto);
        Task<bool> DeleteIssueStatus(long statusId, long projectId, long userId);
        Task<IssueStatusResponseDTO> GetIssueStatusById(long statusId, long userId);
        Task<List<IssueStatusResponseDTO>> GetIssueStatusesByProjectId(long projectId, long userId);
        Task<bool> ReorderStatuses(long projectId, long userId, List<long> statusIds);

        // Issue Management
        Task<IssueResponseDTO> CreateIssue(long userId, CreateIssueRequestDTO dto);
        Task<IssueResponseDTO> UpdateIssue(long issueId, long userId, UpdateIssueRequestDTO dto);
        Task<bool> DeleteIssue(long issueId, long userId);
        Task<IssueResponseDTO> GetIssueById(long issueId, long userId);
        Task<List<IssueResponseDTO>> GetIssuesByProjectId(long projectId, long userId);
        Task<List<IssueResponseDTO>> GetIssuesByStatusId(long statusId, long userId);
        Task<IssueDetailResponseDTO> GetIssueDetail(long issueId, long userId);
        Task<bool> MoveIssue(long issueId, long userId, MoveIssueRequestDTO dto);

        // Project Label Management
        Task<LabelResponseDTO> CreateProjectLabel(long projectId, long userId, CreateProjectLabelRequestDTO dto);
        Task<LabelResponseDTO> UpdateProjectLabel(long labelId, long projectId, long userId, UpdateProjectLabelRequestDTO dto);
        Task<bool> DeleteProjectLabel(long labelId, long projectId, long userId);
        Task<LabelResponseDTO> GetProjectLabelById(long labelId, long userId);
        Task<List<LabelResponseDTO>> GetProjectLabels(long projectId, long userId);

        // Issue Label Management
        Task<bool> AddLabelToIssue(long issueId, long labelId, long userId);
        Task<bool> RemoveLabelFromIssue(long issueId, long labelId, long userId);
        Task<bool> UpdateIssueLabels(long issueId, long userId, List<long> labelIds);

        // Subtask Management
        Task<SubtaskResponseDTO> CreateSubtask(long issueId, long userId, CreateSubtaskRequestDTO dto);
        Task<SubtaskResponseDTO> UpdateSubtask(long subtaskId, long issueId, long userId, UpdateSubtaskRequestDTO dto);
        Task<bool> DeleteSubtask(long subtaskId, long issueId, long userId);
        Task<SubtaskResponseDTO> GetSubtaskById(long subtaskId, long userId);
        Task<List<SubtaskResponseDTO>> GetSubtasksByIssueId(long issueId, long userId);
        Task<bool> ReorderSubtasks(long issueId, long userId, List<long> subtaskIds);

        // Issue History
        Task<List<IssueHistoryResponseDTO>> GetIssueHistory(long issueId, long userId, int limit = 50);
    }
}

