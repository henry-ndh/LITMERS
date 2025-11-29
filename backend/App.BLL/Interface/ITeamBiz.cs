using App.Entity.DTO.Request.Team;
using App.Entity.DTO.Response.Team;
using App.Entity.Models;
using App.Entity.Models.Enums;

namespace App.BLL.Interface
{
    public interface ITeamBiz
    {
        // Team Management
        Task<TeamResponseDTO> CreateTeam(long userId, CreateTeamRequestDTO dto);
        Task<TeamResponseDTO> UpdateTeam(long teamId, long userId, UpdateTeamRequestDTO dto);
        Task<bool> DeleteTeam(long teamId, long userId);
        Task<TeamResponseDTO> GetTeamById(long teamId, long userId);
        Task<List<TeamResponseDTO>> GetMyTeams(long userId);
        Task<TeamDetailResponseDTO> GetTeamDetail(long teamId, long userId);

        // Team Members Management
        Task<bool> RemoveMember(long teamId, long memberId, long requestUserId);
        Task<TeamMemberResponseDTO> UpdateMemberRole(long teamId, long memberId, long requestUserId, UpdateMemberRoleRequestDTO dto);
        Task<List<TeamMemberResponseDTO>> GetTeamMembers(long teamId, long userId);

        // Team Invites Management
        Task<TeamInviteResponseDTO> InviteMember(long teamId, long userId, InviteMemberRequestDTO dto);
        Task<bool> AcceptInvite(string token, long userId);
        Task<bool> CancelInvite(long inviteId, long teamId, long userId);
        Task<List<TeamInviteResponseDTO>> GetMyInvites(string email);

        // Team Activity Logs
        Task<List<TeamActivityLogResponseDTO>> GetActivityLogs(long teamId, long userId, int limit = 50);
    }
}
