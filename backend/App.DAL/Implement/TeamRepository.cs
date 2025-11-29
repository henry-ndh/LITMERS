using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request.Team;
using App.Entity.DTO.Response.Team;
using App.Entity.Models;
using App.Entity.Models.Enums;
using Base.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.DAL.Implement
{
    public class TeamRepository : AppBaseRepository, ITeamRepository
    {
        private readonly BaseDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;

        public TeamRepository(BaseDBContext dbContext, IConfiguration configuration, IEmailService emailService, INotificationRepository notificationRepository) : base(dbContext)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
            _notificationRepository = notificationRepository;
        }

        #region Team Management

        public async Task<TeamModel> CreateTeam(long userId, CreateTeamRequestDTO dto)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception(Constants.UserNotFound);
            }

            var team = new TeamModel
            {
                Name = dto.Name,
                OwnerId = userId,
                CreatedAt = Utils.GetCurrentVNTime(),
                UpdatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<TeamModel>().AddAsync(team);
            await _dbContext.SaveChangesAsync();

            // Add owner as a member with OWNER role
            var teamMember = new TeamMemberModel
            {
                TeamId = team.Id,
                UserId = userId,
                Role = TeamRole.OWNER,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<TeamMemberModel>().AddAsync(teamMember);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await CreateActivityLog(team.Id, userId, ActivityActionType.TEAM_CREATED, team.Id, "Team", $"Created team '{team.Name}'");

            return team;
        }

        public async Task<TeamModel> UpdateTeam(long teamId, long userId, UpdateTeamRequestDTO dto)
        {
            var team = await _dbContext.Set<TeamModel>()
                .Where(t => t.Id == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (team == null)
            {
                throw new Exception("Team not found");
            }

            // Check permission (only OWNER or ADMIN can update)
            if (!await HasPermission(teamId, userId, TeamRole.ADMIN))
            {
                throw new Exception("You don't have permission to update this team");
            }

            var oldName = team.Name;
            team.Name = dto.Name;
            team.UpdatedAt = Utils.GetCurrentVNTime();

            _dbContext.Set<TeamModel>().Update(team);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await CreateActivityLog(teamId, userId, ActivityActionType.TEAM_UPDATED, teamId, "Team", $"Updated team name from '{oldName}' to '{team.Name}'");

            return team;
        }

        public async Task<bool> DeleteTeam(long teamId, long userId)
        {
            var team = await _dbContext.Set<TeamModel>()
                .Where(t => t.Id == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (team == null)
            {
                throw new Exception("Team not found");
            }

            // Only owner can delete team
            if (!await IsTeamOwner(teamId, userId))
            {
                throw new Exception("Only team owner can delete the team");
            }

            team.DeletedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<TeamModel>().Update(team);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await CreateActivityLog(teamId, userId, ActivityActionType.TEAM_DELETED, teamId, "Team", $"Deleted team '{team.Name}'");

            return true;
        }

        public async Task<TeamModel> GetTeamById(long teamId)
        {
            return await _dbContext.Set<TeamModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(t => t.Owner)
                .Where(t => t.Id == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<List<TeamModel>> GetTeamsByUserId(long userId)
        {
            return await _dbContext.Set<TeamMemberModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(tm => tm.Team)
                    .ThenInclude(t => t.Owner)
                .Where(tm => tm.UserId == userId && tm.Team.DeletedAt == null)
                .Select(tm => tm.Team)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TeamDetailResponseDTO> GetTeamDetailById(long teamId, long userId)
        {
            var team = await GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }

            // Check if user is member
            if (!await IsTeamMember(teamId, userId))
            {
                throw new Exception("You are not a member of this team");
            }

            var members = await GetTeamMembers(teamId);
            var pendingInvites = await GetPendingInvites(teamId);
            var userRole = await GetUserRoleInTeam(teamId, userId);

            return new TeamDetailResponseDTO
            {
                Id = team.Id,
                Name = team.Name,
                OwnerId = team.OwnerId,
                OwnerName = team.Owner.Name,
                OwnerEmail = team.Owner.Email,
                CurrentUserRole = userRole,
                CreatedAt = team.CreatedAt,
                UpdatedAt = team.UpdatedAt,
                Members = members,
                PendingInvites = pendingInvites
            };
        }

        #endregion

        #region Team Members Management

        public async Task<TeamMemberModel> AddMember(long teamId, long userId, TeamRole role)
        {
            // Check if already a member
            var existingMember = await GetTeamMember(teamId, userId);
            if (existingMember != null)
            {
                throw new Exception("User is already a member of this team");
            }

            var teamMember = new TeamMemberModel
            {
                TeamId = teamId,
                UserId = userId,
                Role = role,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<TeamMemberModel>().AddAsync(teamMember);
            await _dbContext.SaveChangesAsync();

            return teamMember;
        }

        public async Task<bool> RemoveMember(long teamId, long memberId, long requestUserId)
        {
            var member = await _dbContext.Set<TeamMemberModel>()
                .Include(tm => tm.User)
                .Where(tm => tm.Id == memberId && tm.TeamId == teamId)
                .FirstOrDefaultAsync();

            if (member == null)
            {
                throw new Exception("Member not found");
            }

            // Cannot remove owner
            if (member.Role == TeamRole.OWNER)
            {
                throw new Exception("Cannot remove team owner");
            }

            // Check permission: ADMIN can remove MEMBER, OWNER can remove anyone
            var requestUserRole = await GetUserRoleInTeam(teamId, requestUserId);
            if (requestUserRole == null)
            {
                throw new Exception("You are not a member of this team");
            }

            // Allow removing yourself
            if (member.UserId != requestUserId)
            {
                // Otherwise need permission
                if (requestUserRole == TeamRole.MEMBER)
                {
                    throw new Exception("You don't have permission to remove members");
                }

                if (requestUserRole == TeamRole.ADMIN && member.Role == TeamRole.ADMIN)
                {
                    throw new Exception("Admin cannot remove another admin");
                }
            }

            _dbContext.Set<TeamMemberModel>().Remove(member);
            await _dbContext.SaveChangesAsync();

            // Log activity
            var actionType = member.UserId == requestUserId ? ActivityActionType.MEMBER_LEFT : ActivityActionType.MEMBER_KICKED;
            var message = member.UserId == requestUserId
                ? $"{member.User.Name} left the team"
                : $"Removed {member.User.Name} from the team";

            await CreateActivityLog(teamId, requestUserId, actionType, member.UserId, "User", message);

            return true;
        }

        public async Task<TeamMemberModel> UpdateMemberRole(long teamId, long memberId, long requestUserId, TeamRole newRole)
        {
            var member = await _dbContext.Set<TeamMemberModel>()
                .Include(tm => tm.User)
                .Where(tm => tm.Id == memberId && tm.TeamId == teamId)
                .FirstOrDefaultAsync();

            if (member == null)
            {
                throw new Exception("Member not found");
            }

            // Cannot change owner role
            if (member.Role == TeamRole.OWNER || newRole == TeamRole.OWNER)
            {
                throw new Exception("Cannot change owner role");
            }

            // Only OWNER or ADMIN can change roles
            if (!await HasPermission(teamId, requestUserId, TeamRole.ADMIN))
            {
                throw new Exception("You don't have permission to change member roles");
            }

            var oldRole = member.Role;
            member.Role = newRole;

            _dbContext.Set<TeamMemberModel>().Update(member);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await CreateActivityLog(teamId, requestUserId, ActivityActionType.ROLE_CHANGED, member.UserId, "User",
                $"Changed {member.User.Name}'s role from {oldRole} to {newRole}");

            // Notify member about role change
            var team = await GetTeamById(teamId);
            var requester = await _dbContext.Users.FindAsync(requestUserId);
            var payload = System.Text.Json.JsonSerializer.Serialize(new { teamId = teamId, teamName = team?.Name, newRole = newRole.ToString() });
            await _notificationRepository.CreateNotification(
                member.UserId,
                App.Entity.Models.Enums.NotificationType.TEAM_ROLE_CHANGED,
                $"Your role changed in team: {team?.Name}",
                $"{requester?.Name} changed your role from {oldRole} to {newRole} in team '{team?.Name}'",
                payload
            );

            return member;
        }

        public async Task<List<TeamMemberResponseDTO>> GetTeamMembers(long teamId)
        {
            return await _dbContext.Set<TeamMemberModel>()
                .AsNoTracking() // Performance: No tracking for read-only
                .Include(tm => tm.User)
                .Where(tm => tm.TeamId == teamId)
                .OrderBy(tm => tm.Role)
                .ThenBy(tm => tm.CreatedAt)
                .Select(tm => new TeamMemberResponseDTO
                {
                    Id = tm.Id,
                    UserId = tm.UserId,
                    Name = tm.User.Name,
                    Email = tm.User.Email,
                    Avatar = tm.User.Avatar,
                    Role = tm.Role,
                    JoinedAt = tm.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TeamMemberModel> GetTeamMember(long teamId, long userId)
        {
            return await _dbContext.Set<TeamMemberModel>()
                .Where(tm => tm.TeamId == teamId && tm.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsTeamMember(long teamId, long userId)
        {
            return await _dbContext.Set<TeamMemberModel>()
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<TeamRole?> GetUserRoleInTeam(long teamId, long userId)
        {
            var member = await GetTeamMember(teamId, userId);
            return member?.Role;
        }

        #endregion

        #region Team Invites Management

        public async Task<TeamInviteModel> CreateInvite(long teamId, long userId, string email)
        {
            // Check if user has permission (OWNER or ADMIN)
            if (!await HasPermission(teamId, userId, TeamRole.ADMIN))
            {
                throw new Exception("You don't have permission to invite members");
            }

            // Check if email already in team
            var existingMember = await _dbContext.Set<TeamMemberModel>()
                .Include(tm => tm.User)
                .Where(tm => tm.TeamId == teamId && tm.User.Email == email)
                .FirstOrDefaultAsync();

            if (existingMember != null)
            {
                throw new Exception("User is already a member of this team");
            }

            // Check if there's already a pending invite
            var existingInvite = await _dbContext.Set<TeamInviteModel>()
                .Where(ti => ti.TeamId == teamId && ti.Email == email && ti.AcceptedAt == null && ti.ExpiresAt > Utils.GetCurrentVNTime())
                .FirstOrDefaultAsync();

            if (existingInvite != null)
            {
                throw new Exception("An invite has already been sent to this email");
            }

            var token = Guid.NewGuid().ToString();
            var invite = new TeamInviteModel
            {
                TeamId = teamId,
                Email = email,
                Token = token,
                ExpiresAt = Utils.GetCurrentVNTime().AddDays(7),
                CreatedBy = userId,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<TeamInviteModel>().AddAsync(invite);
            await _dbContext.SaveChangesAsync();

            // Send invite email
            var team = await GetTeamById(teamId);
            var inviter = await _dbContext.Users.FindAsync(userId);

            // Notify user if they have an account (check by email)
            var invitedUser = await _dbContext.Users
                .Where(u => u.Email == email && u.IsActive)
                .FirstOrDefaultAsync();

            if (invitedUser != null)
            {
                var payload = System.Text.Json.JsonSerializer.Serialize(new { teamId = teamId, teamName = team.Name, inviteId = invite.Id, token = invite.Token });
                await _notificationRepository.CreateNotification(
                    invitedUser.Id,
                    App.Entity.Models.Enums.NotificationType.TEAM_INVITE,
                    $"Team invitation: {team.Name}",
                    $"{inviter?.Name} invited you to join team '{team.Name}'",
                    payload
                );
            }

            try
            {
                var clientURL = _configuration["ClientURL"];
                var inviteLink = $"{clientURL}/team/accept-invite?token={token}";

                var placeholders = new Dictionary<string, string>
                {
                    { "InviterName", inviter.Name },
                    { "TeamName", team.Name },
                    { "InviteLink", inviteLink }
                };

                await _emailService.SendEmailAsync(email, "Team Invitation", "team-invite.html", placeholders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send invite email: {ex.Message}");
            }

            // Log activity
            await CreateActivityLog(teamId, userId, ActivityActionType.MEMBER_INVITED, null, "User", $"Invited {email} to the team");

            return invite;
        }

        public async Task<TeamInviteModel> AcceptInvite(string token, long userId)
        {
            var invite = await GetInviteByToken(token);
            if (invite == null)
            {
                throw new Exception("Invalid invite token");
            }

            if (invite.AcceptedAt != null)
            {
                throw new Exception("This invite has already been accepted");
            }

            if (invite.ExpiresAt < Utils.GetCurrentVNTime())
            {
                throw new Exception("This invite has expired");
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null || user.Email != invite.Email)
            {
                throw new Exception("This invite was sent to a different email address");
            }

            // Check if already a member
            if (await IsTeamMember(invite.TeamId, userId))
            {
                throw new Exception("You are already a member of this team");
            }

            // Add user as member
            await AddMember(invite.TeamId, userId, TeamRole.MEMBER);

            // Mark invite as accepted
            invite.AcceptedAt = Utils.GetCurrentVNTime();
            _dbContext.Set<TeamInviteModel>().Update(invite);
            await _dbContext.SaveChangesAsync();

            // Log activity
            await CreateActivityLog(invite.TeamId, userId, ActivityActionType.MEMBER_JOINED, userId, "User", $"{user.Name} joined the team");

            return invite;
        }

        public async Task<bool> CancelInvite(long inviteId, long teamId, long userId)
        {
            var invite = await _dbContext.Set<TeamInviteModel>()
                .Where(ti => ti.Id == inviteId && ti.TeamId == teamId)
                .FirstOrDefaultAsync();

            if (invite == null)
            {
                throw new Exception("Invite not found");
            }

            // Check permission
            if (!await HasPermission(teamId, userId, TeamRole.ADMIN))
            {
                throw new Exception("You don't have permission to cancel invites");
            }

            _dbContext.Set<TeamInviteModel>().Remove(invite);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<TeamInviteResponseDTO>> GetPendingInvites(long teamId)
        {
            return await _dbContext.Set<TeamInviteModel>()
                .Include(ti => ti.Team)
                .Include(ti => ti.Creator)
                .Where(ti => ti.TeamId == teamId && ti.AcceptedAt == null)
                .OrderByDescending(ti => ti.CreatedAt)
                .Select(ti => new TeamInviteResponseDTO
                {
                    Id = ti.Id,
                    TeamId = ti.TeamId,
                    TeamName = ti.Team.Name,
                    Email = ti.Email,
                    Token = ti.Token,
                    ExpiresAt = ti.ExpiresAt,
                    AcceptedAt = ti.AcceptedAt,
                    CreatedBy = ti.CreatedBy,
                    CreatedByName = ti.Creator.Name,
                    CreatedAt = ti.CreatedAt,
                    IsExpired = ti.ExpiresAt < Utils.GetCurrentVNTime(),
                    IsAccepted = ti.AcceptedAt != null
                })
                .ToListAsync();
        }

        public async Task<List<TeamInviteResponseDTO>> GetUserPendingInvites(string email)
        {
            return await _dbContext.Set<TeamInviteModel>()
                .Include(ti => ti.Team)
                .Include(ti => ti.Creator)
                .Where(ti => ti.Email == email && ti.AcceptedAt == null && ti.ExpiresAt > Utils.GetCurrentVNTime())
                .OrderByDescending(ti => ti.CreatedAt)
                .Select(ti => new TeamInviteResponseDTO
                {
                    Id = ti.Id,
                    TeamId = ti.TeamId,
                    TeamName = ti.Team.Name,
                    Email = ti.Email,
                    Token = ti.Token,
                    ExpiresAt = ti.ExpiresAt,
                    AcceptedAt = ti.AcceptedAt,
                    CreatedBy = ti.CreatedBy,
                    CreatedByName = ti.Creator.Name,
                    CreatedAt = ti.CreatedAt,
                    IsExpired = ti.ExpiresAt < Utils.GetCurrentVNTime(),
                    IsAccepted = ti.AcceptedAt != null
                })
                .ToListAsync();
        }

        public async Task<TeamInviteModel> GetInviteByToken(string token)
        {
            return await _dbContext.Set<TeamInviteModel>()
                .Include(ti => ti.Team)
                .Include(ti => ti.Creator)
                .Where(ti => ti.Token == token)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Team Activity Logs

        public async Task<TeamActivityLogModel> CreateActivityLog(long teamId, long actorId, ActivityActionType actionType, long? targetId = null, string targetType = null, string message = null, string metadata = null)
        {
            var log = new TeamActivityLogModel
            {
                TeamId = teamId,
                ActorId = actorId,
                ActionType = actionType,
                TargetId = targetId,
                TargetType = targetType,
                Message = message,
                Metadata = metadata,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<TeamActivityLogModel>().AddAsync(log);
            await _dbContext.SaveChangesAsync();

            return log;
        }

        public async Task<List<TeamActivityLogResponseDTO>> GetActivityLogs(long teamId, int limit = 50)
        {
            return await _dbContext.Set<TeamActivityLogModel>()
                .Include(tal => tal.Actor)
                .Where(tal => tal.TeamId == teamId)
                .OrderByDescending(tal => tal.CreatedAt)
                .Take(limit)
                .Select(tal => new TeamActivityLogResponseDTO
                {
                    Id = tal.Id,
                    TeamId = tal.TeamId,
                    ActorId = tal.ActorId,
                    ActorName = tal.Actor.Name,
                    ActorAvatar = tal.Actor.Avatar,
                    ActionType = tal.ActionType,
                    ActionTypeName = tal.ActionType.ToString(),
                    TargetId = tal.TargetId,
                    TargetType = tal.TargetType,
                    Message = tal.Message,
                    Metadata = tal.Metadata,
                    CreatedAt = tal.CreatedAt
                })
                .ToListAsync();
        }

        #endregion

        #region Helper Methods

        public async Task<bool> IsTeamOwner(long teamId, long userId)
        {
            var team = await _dbContext.Set<TeamModel>()
                .Where(t => t.Id == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync();

            return team != null && team.OwnerId == userId;
        }

        public async Task<bool> IsTeamAdmin(long teamId, long userId)
        {
            var member = await GetTeamMember(teamId, userId);
            return member != null && (member.Role == TeamRole.OWNER || member.Role == TeamRole.ADMIN);
        }

        public async Task<bool> HasPermission(long teamId, long userId, TeamRole minimumRole)
        {
            var userRole = await GetUserRoleInTeam(teamId, userId);
            if (userRole == null) return false;

            // OWNER > ADMIN > MEMBER
            return userRole.Value <= minimumRole;
        }

        #endregion
    }
}
