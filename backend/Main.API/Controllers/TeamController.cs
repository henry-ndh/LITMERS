using App.BLL.Interface;
using App.Entity.DTO.Request.Team;
using Base.API;
using Base.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : BaseAPIController
    {
        private readonly ITeamBiz _teamBiz;
        private readonly ILogger<TeamController> _logger;

        public TeamController(ITeamBiz teamBiz, ILogger<TeamController> logger)
        {
            _teamBiz = teamBiz;
            _logger = logger;
        }

        #region Team Management

        /// <summary>
        /// Create a new team
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _teamBiz.CreateTeam(UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[CreateTeam] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update team information
        /// </summary>
        [HttpPut("{teamId}")]
        public async Task<IActionResult> UpdateTeam(long teamId, [FromBody] UpdateTeamRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _teamBiz.UpdateTeam(teamId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateTeam] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete a team (soft delete)
        /// </summary>
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(long teamId)
        {
            try
            {
                var result = await _teamBiz.DeleteTeam(teamId, UserId);
                return Success("Team deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[DeleteTeam] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get team by ID
        /// </summary>
        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetTeamById(long teamId)
        {
            try
            {
                var result = await _teamBiz.GetTeamById(teamId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetTeamById] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get all teams that the current user is a member of
        /// </summary>
        [HttpGet("my-teams")]
        public async Task<IActionResult> GetMyTeams()
        {
            try
            {
                var result = await _teamBiz.GetMyTeams(UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetMyTeams] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Get team details with members and pending invites
        /// </summary>
        [HttpGet("{teamId}/detail")]
        public async Task<IActionResult> GetTeamDetail(long teamId)
        {
            try
            {
                var result = await _teamBiz.GetTeamDetail(teamId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetTeamDetail] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion

        #region Team Members Management

        /// <summary>
        /// Get all members of a team
        /// </summary>
        [HttpGet("{teamId}/members")]
        public async Task<IActionResult> GetTeamMembers(long teamId)
        {
            try
            {
                var result = await _teamBiz.GetTeamMembers(teamId, UserId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetTeamMembers] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        /// <summary>
        /// Remove a member from the team
        /// </summary>
        [HttpDelete("{teamId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(long teamId, long memberId)
        {
            try
            {
                var result = await _teamBiz.RemoveMember(teamId, memberId, UserId);
                return Success("Member removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[RemoveMember] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Update a member's role in the team
        /// </summary>
        [HttpPut("{teamId}/members/{memberId}/role")]
        public async Task<IActionResult> UpdateMemberRole(long teamId, long memberId, [FromBody] UpdateMemberRoleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _teamBiz.UpdateMemberRole(teamId, memberId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[UpdateMemberRole] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Leave a team (remove yourself)
        /// </summary>
        [HttpPost("{teamId}/leave")]
        public async Task<IActionResult> LeaveTeam(long teamId)
        {
            try
            {
                // Get current user's member ID
                var members = await _teamBiz.GetTeamMembers(teamId, UserId);
                var currentMember = members.FirstOrDefault(m => m.UserId == UserId);

                if (currentMember == null)
                {
                    return Error("You are not a member of this team");
                }

                var result = await _teamBiz.RemoveMember(teamId, currentMember.Id, UserId);
                return Success("You have left the team");
            }
            catch (Exception ex)
            {
                _logger.LogError("[LeaveTeam] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        #endregion

        #region Team Invites Management

        /// <summary>
        /// Invite a new member to the team by email
        /// </summary>
        [HttpPost("{teamId}/invite")]
        public async Task<IActionResult> InviteMember(long teamId, [FromBody] InviteMemberRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _teamBiz.InviteMember(teamId, UserId, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[InviteMember] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Accept a team invitation
        /// </summary>
        [HttpPost("accept-invite")]
        public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _teamBiz.AcceptInvite(dto.Token, UserId);
                return Success("Invite accepted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[AcceptInvite] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a pending invitation
        /// </summary>
        [HttpDelete("{teamId}/invites/{inviteId}")]
        public async Task<IActionResult> CancelInvite(long teamId, long inviteId)
        {
            try
            {
                var result = await _teamBiz.CancelInvite(inviteId, teamId, UserId);
                return Success("Invite cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("[CancelInvite] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Get all pending invitations for the current user
        /// </summary>
        [HttpGet("my-invites")]
        public async Task<IActionResult> GetMyInvites()
        {
            try
            {
                var result = await _teamBiz.GetMyInvites(UserEmail);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetMyInvites] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion

        #region Team Activity Logs

        /// <summary>
        /// Get activity logs for a team
        /// </summary>
        [HttpGet("{teamId}/activity-logs")]
        public async Task<IActionResult> GetActivityLogs(long teamId, [FromQuery] int limit = 50)
        {
            try
            {
                var result = await _teamBiz.GetActivityLogs(teamId, UserId, limit);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetActivityLogs] {0} {1}", ex.Message, ex.StackTrace);
                return GetError(ex.Message);
            }
        }

        #endregion
    }
}
