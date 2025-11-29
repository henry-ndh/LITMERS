using App.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using TFU.Models.IdentityModels;

namespace TFU.Services
{
	public class TFUAuthorizationHandler : IAuthorizationHandler
	{

		private readonly UserManager<UserDTO> _userManager;
		public TFUAuthorizationHandler(UserManager<UserDTO> userManager)
		{
			_userManager = userManager;
		}
		public Task HandleAsync(AuthorizationHandlerContext context)
		{
			if (!context.User.Identity.IsAuthenticated)
			{
				context.Fail();
				return Task.CompletedTask;
			}
			var user = _userManager.GetUserAsync(context.User).Result;

			var pendingRequirements = context.PendingRequirements.ToList();

			foreach (var requirement in pendingRequirements)
			{
				if (requirement is VerifyEmailRequirement)
				{
					if (!user.EmailConfirmed)
					{
						context.Fail();
						return Task.CompletedTask;
					}
				}
				if (requirement is RolesAuthorizationRequirement)
				{
					var roles = (_userManager.GetRolesAsync(user)).Result;
					var isAdmin = roles.Contains(SystemRoleConstants.ADMIN);
					if (!isAdmin)
					{
						context.Fail();
						return Task.CompletedTask;
					}
				}
				context.Succeed(requirement);
			}

			//TODO: Use the following if targeting a version of
			//.NET Framework older than 4.6:
			//      return Task.FromResult(0);
			return Task.CompletedTask;
		}
	}
}
