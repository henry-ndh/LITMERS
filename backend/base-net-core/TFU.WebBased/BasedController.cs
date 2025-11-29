using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TFU.Models.IdentityModels;

namespace TFU.WebBased
{
	public class BasedController : Controller
	{
		private IHttpContextAccessor _httpContextAccessor;
		protected UserManager<UserDTO> _userManager;
		public BasedController(UserManager<UserDTO> userManager, IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// Get the current logged in user id.
		/// </summary>
		protected long UserId
		{
			get
			{
				var id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
				if (long.TryParse(id, out long userId))
					return userId;
				return 0;
			}
		}

		protected string UserName
		{
			get
			{
				return _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
			}
		}
	}
}