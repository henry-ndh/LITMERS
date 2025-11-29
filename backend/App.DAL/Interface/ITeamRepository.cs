using App.Entity.DTO.Request.Team;
using App.Entity.DTO.Response.Team;
using App.Entity.Models;
using App.Entity.Models.Enums;

namespace App.DAL.Interface
{
    public interface ITeamRepository
    {
        // Team Management
        Task<TeamModel> CreateTeam(long userId, CreateTeamRequestDTO dto);
        Task<TeamModel> UpdateTeam(long teamId, long userId, UpdateTeamRequestDTO dto);
        Task<bool> DeleteTeam(long teamId, long userId);
        Task<TeamModel> GetTeamById(long teamId);
        Task<List<TeamModel>> GetTeamsByUserId(long userId);
        Task<TeamDetailResponseDTO> GetTeamDetailById(long teamId, long userId);

        // Team Members Management
        Task<TeamMemberModel> AddMember(long teamId, long userId, TeamRole role);
        Task<bool> RemoveMember(long teamId, long memberId, long requestUserId);
        Task<TeamMemberModel> UpdateMemberRole(long teamId, long memberId, long requestUserId, TeamRole newRole);
        Task<List<TeamMemberResponseDTO>> GetTeamMembers(long teamId);
        Task<TeamMemberModel> GetTeamMember(long teamId, long userId);
        Task<bool> IsTeamMember(long teamId, long userId);
        Task<TeamRole?> GetUserRoleInTeam(long teamId, long userId);

        // Team Invites Management
        Task<TeamInviteModel> CreateInvite(long teamId, long userId, string email);
        Task<TeamInviteModel> AcceptInvite(string token, long userId);
        Task<bool> CancelInvite(long inviteId, long teamId, long userId);
        Task<List<TeamInviteResponseDTO>> GetPendingInvites(long teamId);
        Task<List<TeamInviteResponseDTO>> GetUserPendingInvites(string email);
        Task<TeamInviteModel> GetInviteByToken(string token);

        // Team Activity Logs
        Task<TeamActivityLogModel> CreateActivityLog(long teamId, long actorId, ActivityActionType actionType, long? targetId = null, string targetType = null, string message = null, string metadata = null);
        Task<List<TeamActivityLogResponseDTO>> GetActivityLogs(long teamId, int limit = 50);

        // Helper methods
        Task<bool> IsTeamOwner(long teamId, long userId);
        Task<bool> IsTeamAdmin(long teamId, long userId);
        Task<bool> HasPermission(long teamId, long userId, TeamRole minimumRole);
    }
}
