using App.BLL.Interface;
using App.DAL.Interface;
using App.Entity.DTO.Request.Team;
using App.Entity.DTO.Response.Team;
using App.Entity.Models;
using App.Entity.Models.Enums;
using AutoMapper;

namespace App.BLL.Implement
{
    public class TeamBiz : ITeamBiz
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public TeamBiz(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        #region Team Management

        public async Task<TeamResponseDTO> CreateTeam(long userId, CreateTeamRequestDTO dto)
        {
            var team = await _teamRepository.CreateTeam(userId, dto);
            return await MapToTeamResponse(team, userId);
        }

        public async Task<TeamResponseDTO> UpdateTeam(long teamId, long userId, UpdateTeamRequestDTO dto)
        {
            var team = await _teamRepository.UpdateTeam(teamId, userId, dto);
            return await MapToTeamResponse(team, userId);
        }

        public async Task<bool> DeleteTeam(long teamId, long userId)
        {
            return await _teamRepository.DeleteTeam(teamId, userId);
        }

        public async Task<TeamResponseDTO> GetTeamById(long teamId, long userId)
        {
            var team = await _teamRepository.GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }

            // Check if user is member
            if (!await _teamRepository.IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            return await MapToTeamResponse(team, userId);
        }

        public async Task<List<TeamResponseDTO>> GetMyTeams(long userId)
        {
            var teams = await _teamRepository.GetTeamsByUserId(userId);
            var result = new List<TeamResponseDTO>();

            foreach (var team in teams)
            {
                result.Add(await MapToTeamResponse(team, userId));
            }

            return result;
        }

        public async Task<TeamDetailResponseDTO> GetTeamDetail(long teamId, long userId)
        {
            return await _teamRepository.GetTeamDetailById(teamId, userId);
        }

        #endregion

        #region Team Members Management

        public async Task<bool> RemoveMember(long teamId, long memberId, long requestUserId)
        {
            return await _teamRepository.RemoveMember(teamId, memberId, requestUserId);
        }

        public async Task<TeamMemberResponseDTO> UpdateMemberRole(long teamId, long memberId, long requestUserId, UpdateMemberRoleRequestDTO dto)
        {
            var member = await _teamRepository.UpdateMemberRole(teamId, memberId, requestUserId, dto.Role);
            return _mapper.Map<TeamMemberResponseDTO>(member);
        }

        public async Task<List<TeamMemberResponseDTO>> GetTeamMembers(long teamId, long userId)
        {
            // Check if user is member
            if (!await _teamRepository.IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            return await _teamRepository.GetTeamMembers(teamId);
        }

        #endregion

        #region Team Invites Management

        public async Task<TeamInviteResponseDTO> InviteMember(long teamId, long userId, InviteMemberRequestDTO dto)
        {
            var invite = await _teamRepository.CreateInvite(teamId, userId, dto.Email);
            return _mapper.Map<TeamInviteResponseDTO>(invite);
        }

        public async Task<bool> AcceptInvite(string token, long userId)
        {
            await _teamRepository.AcceptInvite(token, userId);
            return true;
        }

        public async Task<bool> CancelInvite(long inviteId, long teamId, long userId)
        {
            return await _teamRepository.CancelInvite(inviteId, teamId, userId);
        }

        public async Task<List<TeamInviteResponseDTO>> GetMyInvites(string email)
        {
            return await _teamRepository.GetUserPendingInvites(email);
        }

        #endregion

        #region Team Activity Logs

        public async Task<List<TeamActivityLogResponseDTO>> GetActivityLogs(long teamId, long userId, int limit = 50)
        {
            // Check if user is member
            if (!await _teamRepository.IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            return await _teamRepository.GetActivityLogs(teamId, limit);
        }

        #endregion

        #region Helper Methods

        private async Task<TeamResponseDTO> MapToTeamResponse(TeamModel team, long userId)
        {
            var memberCount = (await _teamRepository.GetTeamMembers(team.Id)).Count;
            var userRole = await _teamRepository.GetUserRoleInTeam(team.Id, userId);

            return new TeamResponseDTO
            {
                Id = team.Id,
                Name = team.Name,
                OwnerId = team.OwnerId,
                OwnerName = team.Owner?.Name,
                OwnerEmail = team.Owner?.Email,
                MemberCount = memberCount,
                CurrentUserRole = userRole,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt
            };
        }

        #endregion
    }
}
